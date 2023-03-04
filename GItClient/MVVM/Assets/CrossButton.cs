using System.Windows;
using System.Windows.Controls;

namespace GItClient.MVVM.Assets
{
    public class CrossButton : Button
    {
        static CrossButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CrossButton),
                   new FrameworkPropertyMetadata(typeof(CrossButton)));
        }
    }
}
