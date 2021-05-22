using System;
using System.Collections.Generic;
using System.Text;

namespace EverFilter.Infra
{
    public static class Util
    {
        public static string BaseSnesPath => @"D:\Games\Consoles\SNES\Full Romset";
        public static string DestinationSnesPath => @"D:\Games\Consoles\SNES\Sorter Output";

        public static string BaseNesPath => @"D:\Games\Consoles\NES\Full ROMset - Nintendo NES\Full - Truckload of Hacks Edition";
        public static string DestinationNesPath => @"D:\Games\Consoles\NES\\Sorter Output";


        public static string[] AllowedExtensions => new string[] { ".zip", ".7z", ".rar" };
        public static string[] BadRoms => new string[]
        {
            "[a",
            "[b",
            "[c",
            "[d",
            "[e",
            "[f",
            "[g",
            "[h",
            "[i",
            "[j",
            "[k",
            "[l",
            "[m",
            "[n",
            "[o",
            "[p",
            "[q",
            "[r",
            "[s",
            "[t",
            "[u",
            "[v",
            "[w",
            "[x",
            "[y",
            "[z"
        };

        public static string[] Translations => new string[]
        {
            "T+Eng",
            "T+Kor",
            "T+Spa",
            "T+Ita",
            "T+Chi",
            "T+Rus",
            "T+Fre",
            "T+Ger",
            "T+Por",
            "T+Thai",
            "T+Fin",
            "T+Swe",
            "T+Pol",
            "T+Bra",
            "T+Gre",
            "T-Eng",
            "T-Kor",
            "T-Spa",
            "T-Ita",
            "T-Chi",
            "T-Rus",
            "T-Fre",
            "T-Ger",
            "T-Por",
            "T-Thai",
            "T-Fin",
            "T-Swe",
            "T-Pol",
            "T-Bra",
            "T-Gre",
            "Retrans"
        };

        public static class Folder
        {
            public static string AllOfficialReleases => "All Official Releases";
            public static string Hacks => "Hacks";
            public static string Unknown => "Unknown";
            public static string Translations => "Translations";
            public static string BroadcastSatellite => "Broadcast Satellite";
        }
    }
}
