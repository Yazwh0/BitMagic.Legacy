using System;
using System.IO;
using System.Threading.Tasks;

namespace BitMagic.Common
{
    public class Project
    {
        public ProjectFile Source { get; } = new ProjectFile();
        public ProjectFile PreProcess { get; } = new ProjectFile();
        public ProjectFile Code { get;  } = new ProjectFile();
        public ProjectFile AssemblerObject { get; } = new ProjectFile();

        public Options Options { get; } = new Options();

        public IMachine Machine { get; }

        public Project(IMachine machine)
        {
            Machine = machine;
        }
    }

    public class ProjectFile
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

    public class Options
    {

    }
}
