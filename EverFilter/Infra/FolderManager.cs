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
            ClearFolders();
            UnsupportedFiles = new List<string>();
        }

        private void ClearFolders()
        {
            //var path = Path.Combine(sorter.DestinationPath, Util.Folder.AllOfficialReleases);
            //File.Delete(path);
            //path = Path.Combine(sorter.DestinationPath, Util.Folder.Translations);
            //File.Delete(path);
            //path = Path.Combine(sorter.DestinationPath, Util.Folder.Hacks);
            //File.Delete(path);
            //path = Path.Combine(sorter.DestinationPath, Util.Folder.Unknown);
            //File.Delete(path);
        }

        public bool Extract()
        {
            UnsupportedFiles.Clear();

            if (Directory.Exists(basePath))
                ClearFolders();
            else
                Directory.CreateDirectory(basePath);

            var files = Directory.GetFiles(Path.GetFullPath(basePath));

            bool isCompacted = Util.AllowedExtensions.Any(e => e.Equals(Path.GetExtension(files.FirstOrDefault())));

            if (isCompacted)
            {
                Parallel.ForEach(files, filePath =>
                {
                    OpenArchive(filePath);
                });

                sorter.HandleSpecialCases();
            }
            //else

            return UnsupportedFiles.Count > 0; 
        }

        private void OpenArchive(string filePath)
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
                UnsupportedFiles.Add(filePath); 
            }
        }
    }
}
