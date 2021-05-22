using EverFilter.Infra;
using EverFilter.Sorters;
using System;
using System.Diagnostics;

namespace EverFilter
{
    class Program
    {
        static Stopwatch stopwatch = new Stopwatch();

        static void Main(string[] args)
        {
            var choice = Menu();

            var manager = choice == 1 ? new FolderManager(Util.BaseNesPath, new NesSorter(Util.DestinationNesPath)) 
                : new FolderManager(Util.BaseSnesPath, new SnesSorter(Util.DestinationSnesPath));

            Console.WriteLine("Extracting, please wait...");
            stopwatch.Start();
            manager.Extract();
            stopwatch.Stop();
            Console.WriteLine($"All files extracted and sorted in {stopwatch.Elapsed.TotalSeconds / 60} minutes.");
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
