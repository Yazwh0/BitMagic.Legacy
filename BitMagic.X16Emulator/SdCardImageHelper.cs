using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;
using Silk.NET.SDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BitMagic.X16Emulator;

public static class SdCardImageHelper
{
    // Reading means we pull each extension off and process if necessary. So we can add extra utilities here.
    public static Stream ReadFile(string filename, Stream data) =>
        (Path.GetExtension(filename).ToUpper()) switch
        {
            ".BIN" => data,
            ".VHD" => data,
            ".ZIP" => ReadZipFile(Path.GetFileNameWithoutExtension(filename), data),
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
                "BIN" => data,
                "VHD" => data,
                "ZIP" => CompressZip(data, String.Join('.', parts.Take(index-1))),
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
}

public class SdCardFileNotFoundInZipException : Exception
{
    public SdCardFileNotFoundInZipException(string filename) : base($"Cannot find '{filename}' inside the archive.")
    {
    }
}
