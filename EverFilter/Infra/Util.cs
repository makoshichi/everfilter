using System;
using System.Collections.Generic;
using System.Text;

namespace EverFilter.Infra
{
    public static class Util
    {
        public static string BasePath => @"D:\Games\Consoles\SNES\Full Romset";
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

        public static string[] IgnoredTranslations => new string[]
        {
            "T+Kor",
            "T+Spa",
            "T+Ita",
            "T+Chi",
            "T+Rus",
            "T+Fre",
            "T+Ger",
            "T+Por",
            "T-Kor",
            "T-Spa",
            "T-Ita",
            "T-Chi",
            "T-Rus",
            "T-Fre",
            "T-Ger",
            "T-Por"
        };

        public static string[] DesiredTranslations => new string[]
        {
            "T+Eng",
            "T-Eng"
        };
    }
}
