using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core
{
    internal static class Helper
    {

        private const int MAX_TEXT_LENGTH = 25;

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

    }
}
