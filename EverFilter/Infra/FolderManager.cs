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
        ISorter sorter;
        string path;

        public List<string> UnsupportedFiles { get; private set; }

        public FolderManager(string path, ISorter sorter)
        {
            this.sorter = sorter;
            this.path = path;
            UnsupportedFiles = new List<string>();
        }

        public bool Extract()
        {
            if (!Directory.Exists(path))
                return false;

            bool isSuccess = true;

            var files = Directory.GetFiles(Path.GetFullPath(path));

            Parallel.ForEach(files, filePath =>
            {
                OpenArchive(ref isSuccess, filePath);
            });

            //foreach (var filePath in files)
            //{
            //    OpenArchive(ref isSuccess, filePath);
            //}

            return isSuccess;
        }

        private void OpenArchive(ref bool isSuccess, string filePath)
        {
            if (Util.AllowedExtensions.Any(e => e.Equals(Path.GetExtension(filePath))))
            {
                using (ArchiveFile archive = new ArchiveFile(filePath))
                {
                    sorter.Execute(archive);
                }
            }
            else
            {
                isSuccess = false;
                UnsupportedFiles.Add(filePath);
            }
        }
    }
}
