using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.Text;

namespace EverFilter
{
    public interface ISorter
    {
        void Execute(ArchiveFile archive);
    }
}
