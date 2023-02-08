//
// Copyright (c) 2008-2011, Kenneth Bell
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DiscUtils.Fat;
using DiscUtils.Streams;
using DiscUtils.Streams.Compatibility;

namespace DiscUtils.Fat;

internal class FatBuffer
{
    /// <summary>
    /// The End-of-chain marker to WRITE (SetNext).  Don't use this value to test for end of chain.
    /// </summary>
    /// <remarks>
    /// The actual end-of-chain marker bits on disk vary by FAT type, and can end ...F8 through ...FF.
    /// </remarks>
    public const uint EndOfChain = 0xFFFFFFFF;

    /// <summary>
    /// The Bad-Cluster marker to WRITE (SetNext).  Don't use this value to test for bad clusters.
    /// </summary>
    /// <remarks>
    /// The actual bad-cluster marker bits on disk vary by FAT type.
    /// </remarks>
    public const uint BadCluster = 0xFFFFFFF7;

    /// <summary>
    /// The Free-Cluster marker to WRITE (SetNext).  Don't use this value to test for free clusters.
    /// </summary>
    /// <remarks>
    /// The actual free-cluster marker bits on disk vary by FAT type.
    /// </remarks>
    public const uint FreeCluster = 0;

    private const uint DirtyRegionSize = 512;
    private readonly byte[] _buffer;
    private readonly Dictionary<uint, uint> _dirtySectors;

    private readonly FatType _type;
    private uint _nextFreeCandidate;

    public FatBuffer(FatType type, byte[] buffer)
    {
        _type = type;
        _buffer = buffer;
        _dirtySectors = new Dictionary<uint, uint>();
    }

    internal int NumEntries
    {
        get
        {
            return _type switch
            {
                FatType.Fat12 => _buffer.Length / 3 * 2,
                FatType.Fat16 => _buffer.Length / 2,
                // FAT32
                _ => _buffer.Length / 4,
            };
        }
    }

    internal int Size
    {
        get { return _buffer.Length; }
    }

    internal static bool IsFree(uint val)
    {
        return val == 0;
    }

    internal bool IsEndOfChain(uint val)
    {
        return _type switch
        {
            FatType.Fat12 => (val & 0x0FFF) >= 0x0FF8,
            FatType.Fat16 => (val & 0xFFFF) >= 0xFFF8,
            FatType.Fat32 => (val & 0x0FFFFFF8) >= 0x0FFFFFF8,
            _ => throw new ArgumentException("Unknown FAT type"),
        };
    }

    internal bool IsBadCluster(uint val)
    {
        return _type switch
        {
            FatType.Fat12 => (val & 0x0FFF) == 0x0FF7,
            FatType.Fat16 => (val & 0xFFFF) == 0xFFF7,
            FatType.Fat32 => (val & 0x0FFFFFF8) == 0x0FFFFFF7,
            _ => throw new ArgumentException("Unknown FAT type"),
        };
    }

    internal uint GetNext(uint cluster)
    {
        if (_type == FatType.Fat16)
        {
            return EndianUtilities.ToUInt16LittleEndian(_buffer, (int)(cluster * 2));
        }
        if (_type == FatType.Fat32)
        {
            return EndianUtilities.ToUInt32LittleEndian(_buffer, (int)(cluster * 4)) & 0x0FFFFFFF;
        }

        // FAT12
        if ((cluster & 1) != 0)
        {
            return
                (uint)((EndianUtilities.ToUInt16LittleEndian(_buffer, (int)(cluster + cluster / 2)) >> 4) & 0x0FFF);
        }
        return (uint)(EndianUtilities.ToUInt16LittleEndian(_buffer, (int)(cluster + cluster / 2)) & 0x0FFF);
    }

    internal void SetEndOfChain(uint cluster)
    {
        SetNext(cluster, EndOfChain);
    }

    internal void SetBadCluster(uint cluster)
    {
        SetNext(cluster, BadCluster);
    }

    internal void SetFree(uint cluster)
    {
        if (cluster < _nextFreeCandidate)
        {
            _nextFreeCandidate = cluster;
        }

        SetNext(cluster, FreeCluster);
    }

    internal void SetNext(uint cluster, uint next)
    {
        if (_type == FatType.Fat16)
        {
            MarkDirty(cluster * 2);
            EndianUtilities.WriteBytesLittleEndian((ushort)next, _buffer, (int)(cluster * 2));
        }
        else if (_type == FatType.Fat32)
        {
            MarkDirty(cluster * 4);
            var oldVal = EndianUtilities.ToUInt32LittleEndian(_buffer, (int)(cluster * 4));
            var newVal = (oldVal & 0xF0000000) | (next & 0x0FFFFFFF);
            EndianUtilities.WriteBytesLittleEndian(newVal, _buffer, (int)(cluster * 4));
        }
        else
        {
            var offset = cluster + cluster / 2;
            MarkDirty(offset);
            MarkDirty(offset + 1); // On alternate sector boundaries, cluster info crosses two sectors

            ushort maskedOldVal;
            if ((cluster & 1) != 0)
            {
                next = next << 4;
                maskedOldVal = (ushort)(EndianUtilities.ToUInt16LittleEndian(_buffer, (int)offset) & 0x000F);
            }
            else
            {
                next = next & 0x0FFF;
                maskedOldVal = (ushort)(EndianUtilities.ToUInt16LittleEndian(_buffer, (int)offset) & 0xF000);
            }

            var newVal = (ushort)(maskedOldVal | next);

            EndianUtilities.WriteBytesLittleEndian(newVal, _buffer, (int)offset);
        }
    }

    internal bool TryGetFreeCluster(out uint cluster)
    {
        // Simple scan - don't hold a free list...
        var numEntries = (uint)NumEntries;
        for (uint i = 0; i < numEntries; i++)
        {
            var candidate = (i + _nextFreeCandidate) % numEntries;
            if (IsFree(GetNext(candidate)))
            {
                cluster = candidate;
                _nextFreeCandidate = candidate + 1;
                return true;
            }
        }

        cluster = 0;
        return false;
    }

    internal void FreeChain(uint head)
    {
        foreach (var cluster in GetChain(head).ToArray())
        {
            SetFree(cluster);
        }
    }

    internal IEnumerable<uint> GetChain(uint head)
    {
        if (head != 0)
        {
            var focus = head;
            while (!IsEndOfChain(focus))
            {
                yield return focus;
                focus = GetNext(focus);
            }
        }
    }

    internal void MarkDirty(uint offset)
    {
        _dirtySectors[offset / DirtyRegionSize] = offset / DirtyRegionSize;
    }

    internal void WriteDirtyRegions(Stream stream, long position)
    {
        foreach (var val in _dirtySectors.Values)
        {
            stream.Position = position + val * DirtyRegionSize;
            stream.Write(_buffer, (int)(val * DirtyRegionSize), (int)DirtyRegionSize);
        }
    }

    internal async ValueTask WriteDirtyRegionsAsync(Stream stream, long position, CancellationToken cancellationToken)
    {
        foreach (var val in _dirtySectors.Values)
        {
            stream.Position = position + val * DirtyRegionSize;
            await stream.WriteAsync(_buffer.AsMemory((int)(val * DirtyRegionSize), (int)DirtyRegionSize), cancellationToken).ConfigureAwait(false);
        }
    }

    internal void ClearDirtyRegions()
    {
        _dirtySectors.Clear();
    }
}