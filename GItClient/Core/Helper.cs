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

        private static readonly string[] COLORS = { "#FF5733", "#FF8C00", "#FFFF00", "#00FF00", "#00FFFF", "#0000FF", "#FF00FF", "#FF1493", "#9400D3", "#FFD700", "#008000", "#800080", "#FF69B4", "#00FF7F", "#00BFFF", "#DC143C", "#8B0000", "#9400D3", "#7FFFD4", "#008080", "#808000", "#FFA500", "#7CFC00", "#FFC200", "#FF4500", "#FA8072", "#00FA9A", "#4682B4", "#8FBC8F", "#7B68EE", "#48D1CC", "#B22222", "#191970", "#FF6347", "#BA55D3", "#B0C4DE", "#DAA520", "#E9967A", "#F0E68C", "#DDA0DD", "#FAEBD7" };


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
            //var color = Color.FromRgb((byte)random.Next(100, 250), (byte)random.Next(100, 250), (byte)random.Next(100, 250));
            var index = random.Next(0, COLORS.Length);
            var colorDraw = System.Drawing.ColorTranslator.FromHtml(COLORS[index]);
            var color = Color.FromArgb(colorDraw.A, colorDraw.R, colorDraw.G, colorDraw.B);
            return color;
        }

        public static string GetGeneratedNameFromPath(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? "" : path[(path.LastIndexOf('\\') + 1)..];
        }


    }


}
