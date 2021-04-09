using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EverFilter.Sorters
{
    public abstract class AbstractSorter
    {
        protected string destinationPath;

        public AbstractSorter(string destinationPath)
        {
            this.destinationPath = destinationPath;
        }

        protected IEnumerable<Entry> GetSubset(IEnumerable<Entry> archiveFiles, string region)
        {
            return archiveFiles.Where(rom => rom.FileName.Contains(region)).Select(x => x);
        }

        protected IEnumerable<Entry> GetSubset(IEnumerable<Entry> archiveFiles, Func<Entry, bool> predicate)
        {
            return archiveFiles.Where(predicate).Select(x => x);
        }

        protected bool Ignore(string name, string[] list)
        {
            foreach (var marker in list)
            {
                if (name.Contains(marker))
                    return true;
            }

            return false;
        }

        protected Entry FindLatestVersion(IEnumerable<Entry> archiveFiles, int minor)
        {
            Entry rom = archiveFiles.Where(rom => rom.FileName.Contains($"(V1.{minor})")).FirstOrDefault();
            minor++;
            var version = $"(V1.{minor})"; // Star Fox não possui versão 1.1 USA, não permitindo que a versão 1.2 seja encontrada

            if (archiveFiles.Any(rom => rom.FileName.Contains(version)))
                rom = FindLatestVersion(archiveFiles, minor);

            return rom;
        }

        protected void Save(Entry entry, string folderName)
        {
            var validPath = Path.Combine(destinationPath, folderName);
            if (!Directory.Exists(validPath))
                Directory.CreateDirectory(validPath);

            using (MemoryStream ms = new MemoryStream())
            {
                entry.Extract(ms);
                FileStream file = new FileStream(Path.Combine(validPath, entry.FileName), FileMode.Create);
                ms.WriteTo(file);
                file.Close();
            }
        }
    }
}
