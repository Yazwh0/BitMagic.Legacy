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

namespace BitMagic.X16Emulator;

public unsafe class SdCard : IDisposable
{
    private ulong _memoryPtr;
    private MemoryStream _data;
    private ulong _size;
    private readonly FatFileSystem _fileSystem;
    private ulong _offset;
    private const int Padding = 32;
    private bool _updateable = false; // until we fix the stored image loading
    private FileSystemWatcher? _watcher;

    private string? _homeFolder;
    internal bool _watching = false;

    internal object Lock = new object();
    internal List<string> FileUpdates = new List<string>();

    public SdCard()
    {
        Console.WriteLine($"Creating new SD Card.");
        InitNewCard(32 * 1024 * 1024 + 512);

        var disk = Disk.InitializeFixed(_data, DiscUtils.Streams.Ownership.None, (long)_size - 512);
        BiosPartitionTable.Initialize(disk, WellKnownPartitionType.WindowsFat);

        _fileSystem = FatFileSystem.FormatPartition(disk, 0, "BITMAGIC!", true);
        _updateable = true;
    }

    public SdCard(string sdcardFilename)
    {
        Console.WriteLine($"Loading SD Card from '{sdcardFilename}'.");
        using var fileStream = new FileStream(sdcardFilename, FileMode.Open, FileAccess.Read);
        var stream = SdCardImageHelper.ReadFile(sdcardFilename, fileStream);

        InitNewCard(stream);

        var disk = new Disk(_data, DiscUtils.Streams.Ownership.None);
        _fileSystem = new FatFileSystem(disk.Partitions[0].Open(), true);
    }

    public Thread StartX16Watcher()
    {
        if (string.IsNullOrWhiteSpace(_homeFolder))
            throw new X16FileWatcherButNoHomeFolderException("Cannot watch the X16 SD Card to host without setting a home directory.");

        _watching = true;
        return new Thread(_ => WatchSdCard(this));
    }

    public void StopX16Watcher()
    {
        _watching = false;
    }

    private static void WatchSdCard(SdCard card)
    {
        Dictionary<string, long> previousEntries = card._fileSystem.Root.GetFiles().ToDictionary(i => i.Name, i => i.Length);
        List<DiscFileInfo> files = new List<DiscFileInfo>();

        while (card._watching)
        {
            Thread.Sleep(1000);

            card._fileSystem.UpdateCaches();
            var root = card._fileSystem.Root;

            // compare previous with current to see if there are any changes
            lock (card.Lock)
            {
                card.FileUpdates.Clear();
                files.Clear();

                foreach (var file in root.GetFiles())
                {
                    var writeFile = false;
                    if (previousEntries.ContainsKey(file.Name))
                    {
                        var previous = previousEntries[file.Name];

                        if (previous != file.Length) // todo change to a proper check
                        {
                            // amend!
                            Console.WriteLine($"[X16] >> [PC] Amend: '{file.Name}'");
                            writeFile = true;
                        }
                    }
                    else
                    {
                        // new file!
                        Console.Write($"[X16] >> [PC] New: '{file.Name}'...");
                        writeFile = true;
                    }


                    if (writeFile)
                    {
                        card.FileUpdates.Add(file.Name);
                        var localname = Path.Join(card._homeFolder, file.Name);

                        using var datastream = card._fileSystem.OpenFile(file.Name, FileMode.Open, FileAccess.Read);
                        using var fs = new FileStream(localname, FileMode.OpenOrCreate, FileAccess.Write);

                        datastream.CopyTo(fs);
                        datastream.Close();
                        fs.Close();

                        Console.WriteLine(" Done.");
                    }
                    files.Add(file);
                }
            }

            previousEntries.Clear();
            foreach (var file in files)
                previousEntries.Add(file.Name, file.Length);
        }
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

        stream.Read(rawMemory, (int)_offset, (int)_size);

        _data = new MemoryStream(rawMemory, (int)_offset, (int)_size, true);
    }

    public ulong MemoryPtr => _memoryPtr;

    public void Dispose()
    {
        _watcher?.Dispose();
        _data?.Dispose();
        _fileSystem?.Dispose();
    }

    // Copies a directory and starts a watcher
    public void SetHomeDirectory(string directory, bool hostFsToSdCardSync)
    {
        if (!_updateable)
            throw new CantSyncLoadedImageException("Home directory is currently only available with a new SD Card");

        Console.WriteLine($"Setting home directory to '{directory}'.");
        _homeFolder = directory;

        AddDirectoryFiles(directory);

        if (hostFsToSdCardSync)
        {
            _watcher = new FileSystemWatcher(directory, "*.*");
            _watcher.Changed += _watcher_Changed;
            _watcher.Deleted += _watcher_Deleted;
            _watcher.Created += _watcher_Created;
            _watcher.Renamed += _watcher_Renamed;
            _watcher.Error += _watcher_Error;
            _watcher.EnableRaisingEvents = true;
        }
    }

    private void _watcher_Error(object sender, ErrorEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e.ToString());
        Console.ResetColor();
    }

    private void _watcher_Renamed(object _, RenamedEventArgs e)
    {
        DeleteFile(e.OldFullPath);
        AddFile(e.FullPath);
    }

    private void _watcher_Created(object _, FileSystemEventArgs e) => AddFile(e.FullPath);

    private void _watcher_Deleted(object _, FileSystemEventArgs e) => DeleteFile(e.FullPath);

    private void _watcher_Changed(object _, FileSystemEventArgs e) => AddFile(e.FullPath);

    public void AddDirectory(string directory)
    {
        Console.WriteLine($"Adding files from '{directory}':");
        AddDirectoryFiles(directory);
    }

    private void AddDirectoryFiles(string directory)
    {
        foreach (var filename in System.IO.Directory.GetFiles(directory))
        {
            AddFile(filename);
        }
    }

    public void AddFiles(string filenames)
    {
        var searchName = Path.GetFileName(filenames);
        var path = Path.GetDirectoryName(filenames) ?? throw new Exception("No path!");
        var entries = System.IO.Directory.GetFiles(path, searchName);

        foreach (var filename in entries)
        {
            AddFile(filename);
        }
    }

    private void AddFile(string filename)
    {
        lock (Lock)
        {
            var actName = FixFilename(filename);
            if (FileUpdates.Contains(actName))
            {
                Console.WriteLine($"[PC] >> [16] Skipping : {actName}");
                return;
            }

            Console.Write($"[PC] >> [16] Adding: '{filename}'");
            byte[] source;
            try
            {
                source = File.ReadAllBytes(filename);
            }
            catch (IOException e)
            {
                Console.WriteLine(" Error.");
                Console.WriteLine("Error opening file, trying again in 1s.");
                Thread.Sleep(1000);

                Console.Write($"[PC] >> [16] Adding: '{filename}'");
                source = File.ReadAllBytes(filename);
            }

            Console.Write($" -> '{actName}'...");

            if (_fileSystem.FileExists(actName))
            {
                _fileSystem.DeleteFile(actName);
            }
            using var file = _fileSystem.OpenFile(actName, FileMode.CreateNew, FileAccess.Write);
            file.Write(source);

            file.Close();

            _fileSystem.UpdateFsInfoFreeSpace();

            Console.WriteLine(" Done.");
        }
    }

    private void DeleteFile(string filename)
    {
        lock (Lock)
        {
            var actName = FixFilename(filename);
            if (FileUpdates.Contains(actName))
            {
                Console.WriteLine($"[PC] >> [16] Skipping : {actName}");
                return;
            }

            Console.Write($"[PC] >> [16] Deleteing: '{filename}' -> '{actName}'... ");

            if (_fileSystem.FileExists(actName))
            {
                _fileSystem.DeleteFile(actName);
                _fileSystem.UpdateFsInfoFreeSpace();

                Console.WriteLine(" Done.");
            }
            else
            {
                Console.WriteLine(" Does not exist.");
            }
        }
    }

    public void Save(string filename, bool canOverwrite)
    {
        if (!File.Exists(filename) || canOverwrite)
        {
            Console.Write($"Writing '{filename}'...");
            SdCardImageHelper.WriteFile(filename, _data);
            Console.WriteLine(" Done.");
            return;
        }
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"SD Card image already exists '{filename}'. Nothing saved.");
        Console.ResetColor();
    }

    private static string FixFilename(string filename)
    {
        filename = filename.ToUpper().Replace(" ", "");
        var ext = Path.GetExtension(filename);
        ext = ext[..Math.Min(4, ext.Length)];
        var rawname = Path.GetFileNameWithoutExtension(filename);
        return rawname[..Math.Min(8, rawname.Length)] + ext;
    }
}

public class UnhandledFileSysetmChangeException : Exception
{
    public UnhandledFileSysetmChangeException(string message) : base(message) { }
}

public class CantSyncLoadedImageException : Exception
{
    public CantSyncLoadedImageException(string message) : base(message) { }
}

public class X16FileWatcherButNoHomeFolderException : Exception
{
    public X16FileWatcherButNoHomeFolderException(string message) : base(message) { }
}