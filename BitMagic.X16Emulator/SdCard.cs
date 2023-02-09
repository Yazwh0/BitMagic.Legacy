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
        private MemoryStream _data;
        private ulong _size = 32 * 1024 * 1024; // 32 meg, big enough for anyone.
        private FatFileSystem _fileSystem;
        private ulong _offset;
        private const int Padding = 32;

        public SdCard()
        {
            InitNewCard(32 * 1024 * 1024);

            var disk = Disk.InitializeFixed(_data, DiscUtils.Streams.Ownership.None, (long)_size - 1024*1024);
            BiosPartitionTable.Initialize(disk, WellKnownPartitionType.WindowsFat);

            _fileSystem = FatFileSystem.FormatPartition(disk, 0, "BITMAGIC!", true);
        }

        public SdCard(string sdcardFilename)
        {
            using var fileStream = new FileStream(sdcardFilename, FileMode.Open, FileAccess.Read);
            var stream = SdCardImageHelper.ReadFile(sdcardFilename, fileStream);

            InitNewCard(stream);

            var disk = new Disk(_data, DiscUtils.Streams.Ownership.None);
            _fileSystem = new FatFileSystem(disk.Partitions[0].Open(), true);
        }

        private void InitNewCard(ulong size)
        {
            _size = size;
            var rawMemory = new byte[_size + Padding]; // 32 so we can allign the data correctly
            fixed (byte* ptr = rawMemory)
            {
                _memoryPtr = (ulong)ptr;
                _memoryPtr = (_memoryPtr & ~((ulong)Padding - 1)) + Padding;
                _offset = (_memoryPtr - (ulong)ptr);
            }
            _data = new MemoryStream(rawMemory, (int)_offset, (int)_size, true);
        }

        private void InitNewCard(Stream stream)
        {
            _size = (ulong)stream.Length;
            var rawMemory = new byte[_size + Padding]; // 32 so we can allign the data correctly
            fixed (byte* ptr = rawMemory)
            {
                _memoryPtr = (ulong)ptr;
                _memoryPtr = (_memoryPtr & ~((ulong)Padding - 1)) + Padding;
                _offset = (_memoryPtr - (ulong)ptr);
            }

            stream.Read(rawMemory, (int)_offset, (int)_offset + (int)_size);

            _data = new MemoryStream(rawMemory, (int)_offset, (int)_size, true);
        }

        public ulong MemoryPtr => _memoryPtr;

        public void Dispose()
        {
            //_disk?.Dispose();
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

        public void AddFiles(string filenames)
        {
            var searchName = Path.GetFileName(filenames);
            var path = Path.GetDirectoryName(filenames);
            var entries =  Directory.GetFiles(path, searchName);

            foreach(var filename in entries)
            {
                AddFile(filename);
            }
        }

        private void AddFile(string filename)
        {
            Console.Write($"  Adding '{filename}'");
            var source = File.ReadAllBytes(filename);

            var actName = Path.GetFileName(filename).ToUpper();
            Console.Write($" -> '{actName}'...");

            if (_fileSystem.FileExists(actName))
            {
                _fileSystem.DeleteFile(actName);
            }
            using var file = _fileSystem.OpenFile(actName, FileMode.CreateNew, FileAccess.Write);
            file.Write(source);

            file.Close();
            Console.WriteLine(" Done.");
        }

        public void Save(string filename, bool canOverwrite)
        {
            if (!File.Exists(filename) || canOverwrite) {
                Console.Write($"Writing '{filename}'...");
                SdCardImageHelper.WriteFile(filename, _data);
                Console.WriteLine(" Done.");
                return;
            }
            Console.WriteLine($"SD Card iamge already exists '{filename}'");
        }
    }
}
