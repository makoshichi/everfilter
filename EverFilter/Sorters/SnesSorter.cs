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

        public string DestinationPath => destinationPath;

        public void Execute(ArchiveFile archive)
        {
            var filteredFiles = archive.Entries.Where(rom => !Util.BadRoms.Any(b => rom.FileName.Contains(b)));
            SortTranslations(filteredFiles);

            filteredFiles = filteredFiles.Where(rom => !Util.Translations.Any(t => rom.FileName.Contains(t))).Select(x => x);

            var usa = GetSubset(filteredFiles, GetFilter("(U)", "(JU)"));
            var japan = GetSubset(filteredFiles, GetFilter("(J)", "(ST)"));
            var europe = GetSubset(filteredFiles, rom => rom.FileName.Contains("(E)") && !rom.FileName.Contains("Hack"));
            var hack = GetSubset(filteredFiles, "Hack)");

            SortUnlicensed(hack, Util.Folder.Hacks);

            if (usa.Count() > 0)
            {
                Sort(usa, Util.Folder.AllOfficialReleases);
                return;
            }

            if (europe.Count() > 0)
            {
                Sort(europe, Util.Folder.AllOfficialReleases);
                return;
            }

            if (japan.Count() > 0)
            {
                Insert(japan, Util.Folder.AllOfficialReleases);
                return;
            }

            if (hack.Count() == 0)
                SortUnlicensed(filteredFiles, Util.Folder.Unknown);

            //Task.Factory.StartNew(() =>
            //{
            //    if (hack.Count() == 0)
            //        SortUnlicensed(filteredFiles, Util.Folder.Unknown);
            //});

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
            archiveFiles.ToList().ForEach(entry =>
            {
                var rom = entry;
                CheckForRevision(archiveFiles, ref rom);

                var isBS = rom.FileName.Contains("BS ");

                Save(rom, !isBS ? targetFolder : Path.Combine(targetFolder, Util.Folder.BroadcastSatellite));
            });
        }

        private bool Sort(IEnumerable<Entry> archiveFiles, string targetFolder)
        {
            foreach (var entry in archiveFiles)
            {
                var file = entry;
                CheckForRevision(archiveFiles, ref file);
                Save(file, targetFolder);
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

        object monitor = new object();

        //SLOW
        private void SortUnlicensed(IEnumerable<Entry> archiveFiles, string targetFolder) //Maybe virtual
        {
            //Parallel.ForEach(archiveFiles, entry =>
            //{
            //    if (entry.FileName.Contains("(A&S NES Hack)"))
            //    {
            //        if (!targetFolder.Contains("A&S NES Hack"))
            //            targetFolder = Path.Combine(targetFolder, "A&S NES Hacks");
            //    }

            //    //Save(file, targetFolder);
            //    var validPath = Path.Combine(destinationPath, targetFolder);
            //    if (!Directory.Exists(validPath))
            //        Directory.CreateDirectory(validPath);

            //    lock (monitor)
            //    {
            //        using (MemoryStream ms = new MemoryStream())
            //        {
            //            entry.Extract(ms);
            //            FileStream fs = new FileStream(Path.Combine(validPath, entry.FileName), FileMode.Create);
            //            ms.WriteTo(fs);
            //            fs.Close();
            //        }
            //    }
            //});

            archiveFiles.ToList().ForEach(entry =>
            {
                if (entry.FileName.Contains("(A&S NES Hack)"))
                {
                    if (!targetFolder.Contains("A&S NES Hack"))
                        targetFolder = Path.Combine(targetFolder, "A&S NES Hacks");
                }

                //Save(file, targetFolder);
                var validPath = Path.Combine(destinationPath, targetFolder);
                if (!Directory.Exists(validPath))
                    Directory.CreateDirectory(validPath);

                using (MemoryStream ms = new MemoryStream())
                {
                    entry.Extract(ms);
                    FileStream fs = new FileStream(Path.Combine(validPath, entry.FileName), FileMode.Create);
                    ms.WriteTo(fs);
                    fs.Close();
                }
            });

            //Parallel.ForEach(archiveFiles, entry =>
            //{
            //    if (entry.FileName.Contains("(A&S NES Hack)"))
            //    {
            //        if (!targetFolder.Contains("A&S NES Hack"))
            //            targetFolder = Path.Combine(targetFolder, "A&S NES Hacks");
            //    }

            //    //Save(file, targetFolder);
            //    var validPath = Path.Combine(destinationPath, targetFolder);
            //    if (!Directory.Exists(validPath))
            //        Directory.CreateDirectory(validPath);

            //    return entry;
            //    //using (MemoryStream ms = new MemoryStream())
            //    //{
            //    //    entry.Extract(ms);
            //    //    FileStream fs = new FileStream(Path.Combine(validPath, entry.FileName), FileMode.Create);
            //    //    ms.WriteTo(fs);
            //    //    fs.Close();
            //    //}
            //});

            //archiveFiles.ToList().ForEach(entry =>
            //{
            //    if (entry.FileName.Contains("(A&S NES Hack)"))
            //    {
            //        if (!targetFolder.Contains("A&S NES Hack"))
            //            targetFolder = Path.Combine(targetFolder, "A&S NES Hacks");
            //    }

            //    //Save(file, targetFolder);
            //    var validPath = Path.Combine(destinationPath, targetFolder);
            //    if (!Directory.Exists(validPath))
            //        Directory.CreateDirectory(validPath);

            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        entry.Extract(ms);
            //        FileStream fs = new FileStream(Path.Combine(validPath, entry.FileName), FileMode.Create);
            //        ms.WriteTo(fs);
            //        fs.Close();
            //    }
            //});
        }
    }
}
