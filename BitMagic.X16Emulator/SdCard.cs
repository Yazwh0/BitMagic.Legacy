using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using DiscUtils.Fat;
//using DiscUtils.Raw;
using DiscUtils.Vhd;
using DiscUtils;
using DiscUtils.Core;
using DiscUtils.Partitions;
using System.IO.Enumeration;

namespace BitMagic.X16Emulator
{
    public unsafe class SdCard : IDisposable
    {
        private ulong _memoryPtr; 
        private readonly byte[] _rawMemory;
        private readonly MemoryStream _data;
        private const int _size = 32 * 1024 * 1024; // 32 meg, big enough for anyone.
        private readonly Disk _disk;
        private readonly FatFileSystem _fileSystem;
        private ulong _offset;
        private const ulong Padding = 32;

        public SdCard()
        {
            _rawMemory = new byte[_size + Padding]; // 32 so we can allign the data correctly
            fixed(byte* ptr = _rawMemory)
            {
                _memoryPtr = (ulong)ptr;
                _memoryPtr = (_memoryPtr & ~(Padding-1)) + Padding;
                _offset = (ulong)(_memoryPtr - (ulong)ptr);
            }
            _data = new MemoryStream(_rawMemory, (int)_offset, _size, true);

            _disk = Disk.InitializeFixed(_data, DiscUtils.Streams.Ownership.None, _size - 1024*1024);
            BiosPartitionTable.Initialize(_disk, WellKnownPartitionType.WindowsFat);

            _fileSystem = FatFileSystem.FormatPartition(_disk, 0, "BITMAGIC!", true);
        }

        public ulong MemoryPtr => _memoryPtr;

        public void Dispose()
        {
            _disk?.Dispose();
            _data?.Dispose();
            _fileSystem?.Dispose();
        }

        public void AddDirectory(string directory)
        {
            Console.WriteLine($"Adding files from '{directory}':");
            foreach (var filename in Directory.GetFiles(directory))
            {
                AddFile(filename);
            }
        }

        public void AddFile(string filename)
        {
            Console.Write($"  Adding '{filename}'...");
            var source = File.ReadAllBytes(filename);

            var actName = Path.GetFileName(filename);

            actName = actName.ToUpper();

            using var file = _fileSystem.OpenFile(actName, FileMode.CreateNew, FileAccess.Write);
            file.Write(source);

            file.Close();
            Console.WriteLine(" Done.");
        }

        public void Save(string filename, bool canOverwrite)
        {
            if (!File.Exists(filename) || canOverwrite) {
                Console.Write($"Writing '{filename}'...");
                File.WriteAllBytes(filename, _data.ToArray());
                Console.WriteLine(" Done.");
                return;
            }
            Console.WriteLine($"SD Card iamge already exists '{filename}'");
        }
    }
}
