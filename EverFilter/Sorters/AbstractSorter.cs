using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EverFilter.Sorters
{
    public abstract class AbstractSorter
    {
        protected string destinationPath;

        public AbstractSorter(string destinationPath)
        {
            this.destinationPath = destinationPath;
        }

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
    }
}
