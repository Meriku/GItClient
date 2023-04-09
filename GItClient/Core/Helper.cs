using GItClient.Core.Controllers.Static;
using System;
using System.Linq;
using System.Text;
using System.Windows.Media;

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

        public static bool IsValidFolderName(this string filename)
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
            if (command == CommandsPowerShell.git_Revparse)
            {
                return "git rev-parse";
            }
            return command.ToString().ToLower().Replace('_', ' ');
        }

        public static Color GetRandomColor()
        {
            Random random = new Random();
            var color = Color.FromRgb((byte)random.Next(100, 250), (byte)random.Next(100, 250), (byte)random.Next(100, 250));
            return color;
        }

        public static string GetGeneratedNameFromPath(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? "" : path[(path.LastIndexOf('\\') + 1)..];
        }

    }


}
