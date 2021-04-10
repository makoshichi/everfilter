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
