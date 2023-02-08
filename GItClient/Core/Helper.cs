using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

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

        public static bool IsValidLink(string link)
        {
            return Uri.IsWellFormedUriString(link, UriKind.Absolute);
        }


    }


}
