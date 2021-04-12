using EverFilter.Infra;
using EverFilter.Sorters;
using System;

namespace EverFilter
{
    class Program
    {
        static void Main(string[] args)
        {
            var choice = Menu();

            var manager = choice == 1 ? new FolderManager(Util.BaseNesPath, new NesSorter(Util.DestinationNesPath)) 
                : new FolderManager(Util.BaseSnesPath, new SnesSorter(Util.DestinationSnesPath));
            manager.Extract();
        }

        static int Menu()
        {
            Console.WriteLine("1 - NES");
            Console.WriteLine("2 - SNES");
            var input = Console.ReadLine();

            int choice;
            if (!int.TryParse(input, out choice))
                return Menu();

            if (choice != 1 && choice != 2)
                return Menu();

            return choice;




        }
    }
}
