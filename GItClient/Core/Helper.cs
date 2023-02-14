using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Shapes;
using GItClient.Core.Base;
using GItClient.Core.Controllers;
using Microsoft.Extensions.Logging;

namespace GItClient.Core
{
    internal static class Helper
    {
        private const int MAX_TEXT_LENGTH = 25;
        /// <summary>
        /// Trim text to const lenght
        /// Now is used only in login page
        /// </summary>
        /// <param name="input">Input string to be trimmed</param>
        /// <returns></returns>
        public static string TrimDirectoryName(string input)
        {
            if (input.Length <= MAX_TEXT_LENGTH)
            {
                return input;
            }
            var folders = input.Split('\\');
            var result = new StringBuilder();
            foreach (var folder in folders)
            {
                if (result.Length + folder.Length > MAX_TEXT_LENGTH)
                {
                    return result + "...";
                }
                result.Append(folder + "\\");
            }
            return result.ToString();
        }

        public static bool IsValidLink(this string link)
        {
            return Uri.IsWellFormedUriString(link, UriKind.Absolute);
        }

        public static bool IsValidFilename(this string filename)
        {
            var invalidPathChars = System.IO.Path.GetInvalidFileNameChars();

            if (filename.Any(x => invalidPathChars.Contains(x)))
            {
                return false;
            }
            return true;
        }

        public static string AddBracketsIfSpaces(this string input)
        {
            return input.Contains(' ') ? '"' + input + '"' : input;
        }

        public static string ToPWString(this CommandsPowerShell command)
        {
            return command.ToString().ToLower().Replace('_', ' ');
        }

    }


}
