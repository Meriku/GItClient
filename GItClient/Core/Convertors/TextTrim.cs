using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace GItClient.Core.Convertors
{
    /// <summary>
    /// Calculates text width and trim it
    /// to fit inside the TexBox
    /// </summary>
    public static class TextTrimmer
    {
        private const int PADDING_RIGHT = 20;
        public static string TrimText(TextBox textBox, string originalText)
        {
            var formattedText = GetFormattedText(textBox, originalText);

            if (formattedText.Width < textBox.ActualWidth - PADDING_RIGHT)
            {
                return originalText;
            }

            while (formattedText.Width > textBox.ActualWidth - PADDING_RIGHT && formattedText.Text.Length > 1)
            {
                formattedText = GetFormattedText(textBox, formattedText.Text[..^1]);
            }

            return formattedText.Text + "...";
        }

        private static FormattedText GetFormattedText(TextBox textBox, string text)
        {
            return new FormattedText( text,
               CultureInfo.CurrentCulture,
               FlowDirection.LeftToRight,
               new Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch),
               textBox.FontSize,
               Brushes.Black,
               new NumberSubstitution(), 1);
        }
    }
}
