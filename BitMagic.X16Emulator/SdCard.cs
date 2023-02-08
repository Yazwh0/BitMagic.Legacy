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

            var source = File.ReadAllBytes(@"C:\dev\x4096.prg");

            using var file = _fileSystem.OpenFile("IMAGE.PRG", FileMode.CreateNew, FileAccess.Write);
                        file.Write(source);
            //file.Write(new byte[] { 0x01, 0x80 });
            //file.Write(new byte[] { 0xdb, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            //var a = (byte)1;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { a, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a = 0;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf0, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });

            //a = 0;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf1, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });


            //a = 0;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf2, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });


            //a = 0;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf3, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });


            //a = 0;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });
            //a++;
            //file.Write(new byte[] { 0xf4, a, a, a, a, a, a, a, a, a, a, a, a, a, a, a });

            file.Close();

            //var exists = _fileSystem.FileExists("HELLO.PRG");
            //File.WriteAllBytes(@"c:\dev\mydisk.vhd", _data.ToArray());
        }

        public ulong MemoryPtr => _memoryPtr;

        public void Dispose()
        {
            _disk?.Dispose();
            _data?.Dispose();
            _fileSystem?.Dispose();
        }
    }
}
