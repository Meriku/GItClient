using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;

namespace GItClient.Core.Controllers
{
    /// <summary>
    /// Contoller
    /// Provides services for text
    /// </summary>
    class TextController
    {
        internal Inline[] GetInlinesFromText(string[] text)
        {
            var result = new List<Inline>();
            foreach (var line in text)
            {
                var words = line.Split(' ');
                for (var i = 0; i < words.Length; i++)
                {
                    result.Add(FormatByPattern(words[i], i));
                }
                result.Add(new Run("\n"));
            }
            return result.ToArray();
        }

        internal Run[] GetInlineFromText(string text)
        {
            var result = new List<Run>();
            var words = text.Split(' ');
            for (var i = 0; i < words.Length; i++)
            {
                result.Add(FormatByPattern(words[i], i));
            }
            result.Add(new Run("\n"));
            
            return result.ToArray();
        }

        private Run FormatByPattern(string word, int index)
        {
            word += " ";

            switch (index)
            {
                case 0:
                    return new Run(word) { Foreground = Brushes.DimGray };
                case 1:
                    return new Run(word) { Foreground = Brushes.DarkGoldenrod };
                case 2:
                    return new Run(word) { Foreground = Brushes.SkyBlue };
                case 3:
                    return new Run(word) { Foreground = Brushes.SlateGray };
                default:
                    return new Run(word) { Foreground = Brushes.White };
            }
        }
    }
}
