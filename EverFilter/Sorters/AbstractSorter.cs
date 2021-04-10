using EverFilter.Infra;
using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EverFilter.Sorters
{
    public abstract class AbstractSorter
    {
        protected string destinationPath;

        public AbstractSorter(string destinationPath)
        {
            this.destinationPath = destinationPath;
        }

        protected void SortTranslations(IEnumerable<Entry> archiveFiles)
        {
            var translations = GetSubset(archiveFiles, rom => rom.FileName.Contains("T+Eng") || rom.FileName.Contains("T-Eng"));

            Parallel.ForEach(translations, rom =>
            {
                if (Ignore(rom.FileName, Util.BadRoms))
                    return;
                else
                {
                    if (archiveFiles.Any(rom => rom.FileName.Contains("(V1.0)")))
                        rom = FindLatestVersion(archiveFiles, 0);

                    Save(rom, "Translations");
                }
            });
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
            var version = $"(V1.{minor})"; 

            if (archiveFiles.Any(rom => rom.FileName.Contains(version)))
                rom = FindLatestVersion(archiveFiles, minor);

            return rom;
        }

        // Verificar por que algumas ROMs são salvas com 0 bytes (apenas ocorre com hacks e traduções)
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

        protected abstract void SortUnlicensed(IEnumerable<Entry> archiveFiles, string targetFolder);

        protected abstract void HandleSpecialCases();
    }
}
