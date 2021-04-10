using EverFilter.Infra;
using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EverFilter.Sorters
{
    public class SnesSorter : AbstractSorter, ISorter
    {
        public SnesSorter(string destinationPath) : base(destinationPath)
        {
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

        protected override void SortUnlicensed(IEnumerable<Entry> archiveFiles, string targetFolder)
        {
            Parallel.ForEach(archiveFiles, file =>
            {
                if (Ignore(file.FileName, Util.BadRoms))
                    return;
                else if (Ignore(file.FileName, Util.Translations))
                    return;
                else
                {
                    if (file.FileName.Contains("(A&S NES Hack)"))
                    {
                        if (!targetFolder.Contains("A&S NES Hack"))
                            targetFolder = Path.Combine(targetFolder, "A&S NES Hacks");
                    }

                    Save(file, targetFolder);
                }
            });
        }

        protected override void HandleSpecialCases()
        {
            // Star Fox V1.2
            throw new NotImplementedException();
        }
    }
}
