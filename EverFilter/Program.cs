using EverFilter.Infra;
using EverFilter.Sorters;
using System;

namespace EverFilter
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new FolderManager(Util.BaseSnesPath, new SnesSorter(Util.DestinationSnesPath));
            //var manager = new FolderManager(Util.BaseNesPath, new SnesSorter(Util.DestinationNesPath));
            manager.Extract();
        }
    }
}
