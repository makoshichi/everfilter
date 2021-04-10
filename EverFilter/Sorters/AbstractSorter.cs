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

        public void Execute(ArchiveFile archive)
        {
            var usa = GetSubset(archive.Entries, rom => rom.FileName.Contains("(U)") || rom.FileName.Contains("(JU)"));
            var japan = GetSubset(archive.Entries, rom => rom.FileName.Contains("(J)") || rom.FileName.Contains("(ST)"));
            var europe = GetSubset(archive.Entries, "(E)");
            var hack = GetSubset(archive.Entries, "Hack)");

            bool isUsa, isJapan, isEurope;

            SortUnlicensed(hack, "Hacks");

            isUsa = Sort(usa, "All Official Releases");

            if (isUsa)
                return;

            isEurope = Sort(europe, "All Official Releases");

            if (isEurope)
                return;

            isJapan = Sort(japan, "All Official Releases");

            if (isJapan)
                return;

            if (hack.Count() == 0)
                SortUnlicensed(archive.Entries, "Unknown");
        }

        protected bool Sort(IEnumerable<Entry> archiveFiles, string targetFolder)
        {
            SortTranslations(archiveFiles);

            foreach (var entry in archiveFiles)
            {
                if (Ignore(entry.FileName, Util.BadRoms))
                    continue;
                else if (Ignore(entry.FileName, Util.Translations))
                    continue;
                else
                {
                    var rom = entry;

                    if (archiveFiles.Any(rom => rom.FileName.Contains("(V1.0)")))
                        rom = FindLatestVersion(archiveFiles, 0);

                    Save(rom, targetFolder);
                    return true;
                }
            }

            return false;
        }

        private void SortTranslations(IEnumerable<Entry> archiveFiles)
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
