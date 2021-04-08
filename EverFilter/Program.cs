using EverFilter.Infra;
using EverFilter.Sorters;
using System;

namespace EverFilter
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new FolderManager(Util.BasePath, new SnesSorter());
            manager.Extract();
        }
    }
}
