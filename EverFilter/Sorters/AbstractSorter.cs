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

        protected object lockObj = new object();

        public AbstractSorter(string destinationPath)
        {
            this.destinationPath = destinationPath;
        }

        protected void SortTranslations(IEnumerable<Entry> archiveFiles)
        {
            var translations = GetSubset(archiveFiles, rom => rom.FileName.Contains("T+Eng") || rom.FileName.Contains("T-Eng"));

            translations.ToList().ForEach(rom =>
            {
                if (Ignore(rom.FileName, Util.BadRoms))
                    return;
                else
                {
                    CheckForRevision(archiveFiles, ref rom);
                    Save(rom, Util.Folder.Translations);
                }
            });

            // Talvez não usar parallel.ForEach
            //Parallel.ForEach(translations, rom =>
            //{
            //    if (Ignore(rom.FileName, Util.BadRoms))
            //        return;
            //    else
            //    {
            //        CheckForRevision(archiveFiles, ref rom);
            //        Save(rom, Util.Folder.Translations);
            //    }
            //});
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
            var version = GetVersion(archiveFiles, ref minor, out Entry rom);

            //Entry rom = archiveFiles.Where(rom => rom.FileName.Contains($"(V1.{minor})")).FirstOrDefault();
            //minor++;
            //var version = $"(V1.{minor})";


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

        protected abstract void CheckForRevision(IEnumerable<Entry> archiveFiles, ref Entry rom);

        protected abstract string GetVersion(IEnumerable<Entry> archiveFiles, ref int minor, out Entry rom);

        //protected virtual void SortUnlicensed(IEnumerable<Entry> archiveFiles, string targetFolder){ };
    }
}
