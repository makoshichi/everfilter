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
            var filteredFiles = archive.Entries.Where(rom => !Util.BadRoms.Any(b => rom.FileName.Contains(b)));
            SortTranslations(filteredFiles);

            filteredFiles = filteredFiles.Where(rom => !Util.Translations.Any(t => rom.FileName.Contains(t))).Select(x => x);

            var usa = GetSubset(filteredFiles, GetFilter("(U)", "(JU)"));
            var japan = GetSubset(filteredFiles, GetFilter("(J)", "(ST)"));
            var europe = GetSubset(filteredFiles, rom => rom.FileName.Contains("(E)") && !rom.FileName.Contains("Hack"));
            var hack = GetSubset(filteredFiles, "Hack)");

            SortUnlicensed(hack, "Hacks");

            if (usa.Count() > 0)
            {
                //Insert(usa, "All Official Releases");
                Sort(usa, "All Official Releases");
                return;
            }

            if (europe.Count() > 0)
            {
                //Insert(europe, "All Official Releases");
                Sort(europe, "All Official Releases");
                return;
            }

            if (japan.Count() > 0)
            {
                Insert(japan, "All Official Releases");
                return;
            }

            if (hack.Count() == 0)
                SortUnlicensed(filteredFiles, "Unknown");
        }


        public void HandleSpecialCases()
        {
            // Star Fox V1.2
            // Final Fantasy III (U) (V1.1) (Retrans 2.00a by Sky Render)
        }

        private Func<Entry, bool> GetFilter(string marker1, string marker2)
        {
            return rom => (rom.FileName.Contains(marker1) || rom.FileName.Contains(marker2)) && !rom.FileName.Contains("Hack");
        }

        private void Insert(IEnumerable<Entry> archiveFiles, string targetFolder)
        {
            Parallel.ForEach(archiveFiles, entry =>
            {
                var rom = entry;
                CheckForRevision(archiveFiles, ref rom);

                var isBS = rom.FileName.Contains("BS ");

                Save(rom, !isBS ? targetFolder : Path.Combine(targetFolder, "Broadcast Satellite"));
            });
        }

        private bool Sort(IEnumerable<Entry> archiveFiles, string targetFolder)
        {
            foreach (var entry in archiveFiles)
            {
                var rom = entry;
                CheckForRevision(archiveFiles, ref rom);
                Save(rom, targetFolder);
                return true;
            }
            return false;
        }

        protected override void CheckForRevision(IEnumerable<Entry> archiveFiles, ref Entry rom)
        {
            if (archiveFiles.Any(rom => rom.FileName.Contains("(V1.0)")))
                rom = FindLatestVersion(archiveFiles, 0);
        }

        protected override string GetVersion(IEnumerable<Entry> archiveFiles, ref int minor, out Entry rom)
        {
            int val = minor;
            rom = archiveFiles.Where(rom => rom.FileName.Contains($"(V1.{val})")).FirstOrDefault();
            minor++;
            return $"(V1.{minor})";
        }

        private void SortUnlicensed(IEnumerable<Entry> archiveFiles, string targetFolder) //Maybe virtual
        {
            Parallel.ForEach(archiveFiles, file =>
            {
                //if (Ignore(file.FileName, Util.BadRoms))
                //    return;
                //else if (Ignore(file.FileName, Util.Translations))
                //    return;
                //else
                //{
                    if (file.FileName.Contains("(A&S NES Hack)"))
                    {
                        if (!targetFolder.Contains("A&S NES Hack"))
                            targetFolder = Path.Combine(targetFolder, "A&S NES Hacks");
                    }

                    Save(file, targetFolder);
                //}
            });
        }
    }
}
