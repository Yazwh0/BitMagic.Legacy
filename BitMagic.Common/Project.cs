using System;
using System.IO;
using System.Threading.Tasks;

namespace BitMagic.Common
{
    public class Project
    {
        public ProjectTextFile Source { get; } = new ProjectTextFile();
        public ProjectTextFile PreProcess { get; } = new ProjectTextFile();
        public ProjectTextFile Code { get;  } = new ProjectTextFile();
        public ProjectTextFile AssemblerObject { get; } = new ProjectTextFile();

        public ProjectBinFile ProgFile { get; } = new ProjectBinFile();

        public Options Options { get; }

        public IMachine Machine { get; }

        public TimeSpan LoadTime { get; set; }
        public TimeSpan PreProcessTime { get; set; }
        public TimeSpan CompileTime { get; set; }

        public Project(IMachine machine)
        {
            Machine = machine;
            Options = new Options();
        }
    }

    public class ProjectBinFile
    {
        public string? Filename { get; set; } = null;
        public byte[]? Contents { get; set; } = null;

        public Task Load(string filename)
        {
            Filename = filename;
            return Load();
        }

        public async Task Load()
        {
            if (string.IsNullOrWhiteSpace(Filename))
                throw new ArgumentNullException(nameof(Filename));

            Contents = await File.ReadAllBytesAsync(Filename);
        }

        public Task Save(string fielname)
        {
            Filename = Filename;
            return Save();
        }

        public async Task Save()
        {
            if (string.IsNullOrWhiteSpace(Filename))
                throw new ArgumentNullException(nameof(Filename));

            await File.WriteAllBytesAsync(Filename, Contents);
        }
    }


    public class ProjectTextFile
    {
        public string? Filename { get; set; } = null;
        public string? Contents { get; set; } = null;

        public Task Load(string filename)
        {
            Filename = filename;
            return Load();
        }

        public async Task Load()
        {
            if (string.IsNullOrWhiteSpace(Filename))
                throw new ArgumentNullException(nameof(Filename));

            Contents = await File.ReadAllTextAsync(Filename);
        }

        public Task Save(string fielname)
        {
            Filename = Filename;
            return Save();
        }

        public async Task Save()
        {
            if (string.IsNullOrWhiteSpace(Filename))
                throw new ArgumentNullException(nameof(Filename));

            await File.WriteAllTextAsync(Filename, Contents);
        }
    }

    [Flags]
    public enum ApplicationPart
    {
        None = 0,
        Macro       = 0b0000_0001,
        Compiler    = 0b0000_0010,
        Emulator    = 0b0000_0100
    }

    public class Options
    {
        public ApplicationPart VerboseDebugging { get; set; }
    }
}
