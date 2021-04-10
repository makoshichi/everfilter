using EverFilter.Infra;
using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EverFilter.Sorters
{
    public class NesSorter : AbstractSorter, ISorter
    {
        public NesSorter(string destinationPath) : base(destinationPath)
        {
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
                    Save(file, targetFolder);
                }
            });
        }

        protected override void HandleSpecialCases()
        {
            // Contra (J)
            // Castlevania 3 (J)
            // Solbrain
            // Magic John (Totally Rad)
            throw new NotImplementedException();
        }
    }
}
