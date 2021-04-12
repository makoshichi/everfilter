using EverFilter.Infra;
using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EverFilter.Sorters
{
    public class NesSorter : AbstractSorter, ISorter
    {
        //string[] unlicensed = new string[] { "(Camerica)", "(Wisdom Tree)", "(Unl)" };

        public NesSorter(string destinationPath) : base(destinationPath)
        {
        }

        public void Execute(ArchiveFile archive)
        {
            var usa = GetSubset(archive.Entries, rom => rom.FileName.Contains("(U)") || rom.FileName.Contains("(JU)") || rom.FileName.Contains("(W)"));
            var japan = GetSubset(archive.Entries, "(J)");
            var europe = GetSubset(archive.Entries, "(E)");
            //var hack = GetSubset(archive.Entries, "Hack)");

            bool isUsa, isJapan, isEurope;

            //SortUnlicensed(hack, "Hacks");

            isUsa = Sort(usa, "All Official Releases");

            if (isUsa)
                return;

            isEurope = Sort(europe, "All Official Releases");

            if (isEurope)
                return;

            isJapan = Sort(japan, "All Official Releases");

            if (isJapan)
                return;

            //if (hack.Count() == 0)
            //    SortUnlicensed(archive.Entries, "Unknown");
        }

        public void HandleSpecialCases()
        {
            // Contra (J)
            // Castlevania 3 (J)
            // Gimmick (J)
            // Solbrain
            // Magic John (Totally Rad)
            // Rockman + Mega Man

            SplitInSubfolders("All Official Releases");
            SplitInSubfolders("Hacks");
        }

        private bool Sort(IEnumerable<Entry> archiveFiles, string targetFolder) 
        {
            SortTranslations(archiveFiles);

            foreach (var entry in archiveFiles)
            {
                if (Ignore(entry.FileName, Util.BadRoms))
                    continue;
                if (Ignore(entry.FileName, Util.Translations))
                    continue;
                if (IsPrototype(entry))
                {
                    Save(entry, Path.Combine(destinationPath, "Prototypes"));
                    continue;
                }
                if (IsAllowedHack(entry))
                {
                    var r = entry;
                    CheckForRevision(archiveFiles, ref r);
                    Save(entry, Path.Combine(destinationPath, "Hacks"));
                    continue;

                }

                var rom = entry;
                CheckForRevision(archiveFiles, ref rom);
                Save(rom, targetFolder);
                return true;
            }

            return false;
        }

        private bool IsPrototype(Entry entry)
        {
            return entry.FileName.Contains("(Prototype)");
        }

        private bool IsAllowedHack(Entry entry)
        {
            return entry.FileName.Contains("Hack") && (entry.FileName.Contains("Metroid")
                    || entry.FileName.Contains("Megaman") || entry.FileName.Contains("SMB1"));
        }

        private void SplitInSubfolders(string targetFolder)
        {
            var basePath = Path.Combine(destinationPath, targetFolder);
            var files = new DirectoryInfo(basePath).GetFiles();

            // Ideia base
            var subfolder = string.Empty;

            for (int i = 0; i < files.Length; i++)
            {
                if (i % 254 == 0)
                {
                    var from = files[i].Name.ToCharArray()[0];
                    char to; ;

                    if (int.TryParse(from.ToString(), out int number))
                        from = '#';

                    if (i + 254 > files.Length)
                        to = 'Z';
                    else
                        to = files[i + 254].Name.ToCharArray()[0];

                    subfolder = $"{from} - {to}";
                }

                //if (i % 254 == 0) 
                //    subfolder = $"{i} - {i + 254}";


                //files[i].MoveTo(Path.Combine(basePath, subfolder)); //?
                files[i].MoveTo(Path.Combine(basePath, Path.Combine(subfolder, files[i].Name)));
            }

            //Test
            //Parallel.For(0, files.Length, (i) => {
            //    if (i % 254 == 0)
            //        subfolder = $"{i} - {i + 254}";

            //    files[i].MoveTo(Path.Combine(basePath, subfolder));
            //});
        }

        protected override void CheckForRevision(IEnumerable<Entry> archiveFiles, ref Entry rom)
        {
            if (archiveFiles.Any(rom => rom.FileName.Contains("(PRG0)") || rom.FileName.Contains("(REV")))
                rom = FindLatestVersion(archiveFiles, 0);
        }

        protected override string GetVersion(IEnumerable<Entry> archiveFiles, ref int minor, out Entry rom)
        {
            //PRG#
            //REV#
            throw new NotImplementedException();
        }

        //Max 254 per folder
        //Maybe virtual
        private void SortUnlicensed(IEnumerable<Entry> archiveFiles, string targetFolder)
        {
            Parallel.ForEach(archiveFiles, file =>
            {
                if (Ignore(file.FileName, Util.BadRoms))
                    return;
                else if (Ignore(file.FileName, Util.Translations))
                    return;
                else
                {
                    Save(file, targetFolder);
                }
            });
        }
    }
}
