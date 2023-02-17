using DiscUtils.Streams;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics.Tracing;

namespace BitMagic.X16Emulator;

public static class SdCardImageHelper
{
    // Reading means we pull each extension off and process if necessary. So we can add extra utilities here.
    // todo: handle .bin -> vhd.
    public static Stream ReadFile(string filename, Stream data) =>
        (Path.GetExtension(filename).ToUpper()) switch
        {
            ".BIN" => data,
            ".VHD" => data,
            ".ZIP" => ReadZipFile(Path.GetFileNameWithoutExtension(filename), data),
            ".GZ" => ReadGzFile(Path.GetFileNameWithoutExtension(filename), data),
            _ => data
        };

    private static Stream ReadZipFile(string filename, Stream data)
    {
        var zipStream = new ZipInputStream(data);

        // look through the archive for a file that matches the filename
        var entry = zipStream.GetNextEntry();
        while (entry != null)
        {
            if (entry.IsFile && string.Equals(entry.Name, filename, StringComparison.InvariantCultureIgnoreCase))
            {
                var file = new byte[entry.Size];
                zipStream.Read(file, 0, (int)entry.Size);
                return ReadFile(filename, new MemoryStream(file));
            }

            entry = zipStream.GetNextEntry();
        }

        throw new SdCardFileNotFoundInZipException(filename);
    }

    private static Stream ReadGzFile(string filename, Stream data)
    {
        data.Position = 0;
        var gzStream = new GZipInputStream(data);
            
        if (gzStream.Available == 1)
        {
            var file = new MemoryStream();
            gzStream.CopyTo(file);

            var uncompressedData = file.ToArray();
            var toReturn = new MemoryStream(uncompressedData);

            return ReadFile(filename, toReturn);
        }

        throw new GzCorruptException(filename);
    }

    public static void WriteFile(string filename, Stream data) => WriteFileHelper(filename, Path.GetFileName(filename).Split('.'), 1, data);

    private static void WriteFileHelper(string filename, string[] parts, int index, Stream data)
    {
        while (true)
        {
            if (index >= parts.Length)
            {
                data.Position = 0;
                using var fileStream = File.Create(filename);
                data.CopyTo(fileStream);
                return;
            }

            data = (parts[index++].ToUpper()) switch
            {
                "BIN" => new SubStream(data, Ownership.None, 0, data.Length - 512), // snip vhd footer
                "VHD" => data,
                "ZIP" => CompressZip(data, String.Join('.', parts.Take(index-1))),
                "GZ" => CompressGz(data),
                _ => data
            };
        }
    }

    private static Stream CompressZip(Stream data, string filename)
    {
        var toReturn = new MemoryStream();
        var zipStream = new ZipOutputStream(toReturn);
        zipStream.SetLevel(9);

        var zipEntry = new ZipEntry(filename);

        zipEntry.DateTime = DateTime.Now;
        zipStream.PutNextEntry(zipEntry);

        data.Position = 0;
        data.CopyTo(zipStream);

        zipStream.CloseEntry();
        zipStream.IsStreamOwner = false;
        zipStream.Finish();
        zipStream.Close();

        return toReturn;
    }

    private static Stream CompressGz(Stream data)
    {
        var toReturn = new MemoryStream();
        var gzStream = new GZipOutputStream(toReturn);

        gzStream.SetLevel(9);

        data.Position = 0;
        data.CopyTo(gzStream);

        gzStream.Finish();

        return toReturn;
    }
}

public class SdCardFileNotFoundInZipException : Exception
{
    public SdCardFileNotFoundInZipException(string filename) : base($"Cannot find '{filename}' inside the archive.")
    {
    }
}

public class GzCorruptException : Exception
{
    public GzCorruptException(string filename) : base($"'{filename}' doesn't appear to be a valid .gz file.")
    {
    }
}

public class FsImage
{
    private readonly byte[] _rawData;

    public FsImage(byte[] rawData)
    {
        _rawData = rawData;
    }

    public byte Mounted => _rawData[0];
    public ushort RootDir_Cluster => (ushort)(_rawData[1] + (_rawData[2] << 8));
    public byte Sectors_Per_Cluster => _rawData[3];
    public byte Cluster_Shift => _rawData[4];
    public ushort Lba_Partition => (ushort)(_rawData[5] + (_rawData[6] << 8));
    public ushort Fat_Size => (ushort)(_rawData[7] + (_rawData[8] << 8));
    public ushort Lba_Fat => (ushort)(_rawData[9] + (_rawData[10] << 8));
    public ushort Lba_Data => (ushort)(_rawData[11] + (_rawData[12] << 8));
    public ushort Cluster_Count => (ushort)(_rawData[13] + (_rawData[14] << 8));
    public ushort Lba_FsInfo => (ushort)(_rawData[15] + (_rawData[16] << 8));
    public ushort Free_Clusters => (ushort)(_rawData[17] + (_rawData[18] << 8));
    public ushort Free_Cluster => (ushort)(_rawData[19] + (_rawData[20] << 8));
    public ushort Cwd_Cluster => (ushort)(_rawData[21] + (_rawData[22] << 8));
}