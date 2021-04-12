using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverFilter.Infra
{
    public class FolderManager
    {
        readonly ISorter sorter;
        readonly string basePath;

        public List<string> UnsupportedFiles { get; private set; }

        public FolderManager(string basePath, ISorter sorter)
        {
            this.sorter = sorter;
            this.basePath = basePath;
            UnsupportedFiles = new List<string>();
        }

        public bool Extract()
        {
            UnsupportedFiles.Clear();

            if (!Directory.Exists(basePath))
                return false;

            var files = Directory.GetFiles(Path.GetFullPath(basePath));

            bool isCompacted = Util.AllowedExtensions.Any(e => e.Equals(Path.GetExtension(files.FirstOrDefault())));

            if (isCompacted)
            {
                //foreach (var filePath in files)
                //{
                //    OpenArchive(filePath);
                //}

                Parallel.ForEach(files, filePath =>
                {
                    OpenArchive(filePath);
                });

                sorter.HandleSpecialCases();
            }
            //else

            return UnsupportedFiles.Count > 0; // Pode se tornar obsoleto
        }

        private void OpenArchive(string filePath)
        {
            if (Util.AllowedExtensions.Any(e => e.Equals(Path.GetExtension(filePath)))) // Pode se tornar obsoleto
            {
                using (ArchiveFile archive = new ArchiveFile(filePath))
                {
                    sorter.Execute(archive);
                }
            }
            else // Pode se tornar obsoleto
            {
                UnsupportedFiles.Add(filePath); // Pode se tornar obsoleto
            }
        }
    }
}
