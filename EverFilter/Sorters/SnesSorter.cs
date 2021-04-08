using EverFilter.Infra;
using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EverFilter.Sorters
{
    public class SnesSorter : ISorter
    {
        public void Execute(ArchiveFile archive)
        {
            var usa = GetSubset(archive, rom => rom.FileName.Contains("(U)") || rom.FileName.Contains("(JU)"));
            var japan = GetSubset(archive, rom => rom.FileName.Contains("(J)") || rom.FileName.Contains("(ST)"));
            var europe = GetSubset(archive, "(E)");
            var hack = GetSubset(archive, "Hack)");

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

        private IEnumerable<Entry> GetSubset(ArchiveFile archive, string region)
        {
            return ((List<Entry>)archive.Entries).Where(rom => rom.FileName.Contains(region)).Select(x => x);
        }

        private IEnumerable<Entry> GetSubset(ArchiveFile archive, Func<Entry, bool> predicate)
        {
            return ((List<Entry>)archive.Entries).Where(predicate).Select(x => x);
        }

        private bool Sort(IEnumerable<Entry> list, string targetFolder)
        {
            foreach (var entry in list)
            {
                if (IsIgnore(entry.FileName, Util.BadRoms))
                    continue;
                else if (IsIgnore(entry.FileName, Util.IgnoredTranslations))
                    continue;
                else
                {
                    var rom = entry;

                    if (IsDesiredTranslation(rom.FileName)) //Pega tradução apenas se estiver antes da rom oficial; traduções também deveriam levar a versão em consideração
                    {
                        Save(rom, "Translations");
                        continue;
                    }

                    if (list.Any(rom => rom.FileName.Contains("(V1.0)")))
                        rom = FindLatestVersion(list, 0);

                    Save(rom, targetFolder);
                    return true;
                }
            }

            return false;
        }

        private void SortUnlicensed(IEnumerable<Entry> list, string targetFolder)
        {
            Parallel.ForEach(list, entry =>
            {
                if (IsIgnore(entry.FileName, Util.BadRoms))
                    return;
                else if (IsIgnore(entry.FileName, Util.IgnoredTranslations))
                    return;
                else
                {
                    if (entry.FileName.Contains("(A&S NES Hack)"))
                    {
                        if (!targetFolder.Contains("A&S NES Hack"))
                            targetFolder = Path.Combine(targetFolder, "A&S NES Hacks");
                    }

                    Save(entry, targetFolder);
                }
            });
        }

        private bool IsIgnore(string name, string[] list)
        {
            foreach (var marker in list)
            {
                if (name.Contains(marker))
                    return true;
            }

            return false;
        }

        private bool IsDesiredTranslation(string name)
        {
            return IsIgnore(name, Util.DesiredTranslations);
        }

        private Entry FindLatestVersion(IEnumerable<Entry> archive, int minor)
        {
            Entry rom = archive.Where(rom => rom.FileName.Contains($"(V1.{minor})")).FirstOrDefault();
            minor++;
            var version = $"(V1.{minor})"; // Star Fox não possui versão 1.1 USA, não permitindo que a versão 1.2 seja encontrada

            if (archive.Any(rom => rom.FileName.Contains(version)))
                rom = FindLatestVersion(archive, minor);

            return rom;
        }

        private void Save(Entry entry, string folderName)
        {
            var validPath = Path.Combine(Util.BasePath, folderName);
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
