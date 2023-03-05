using GItClient.MVVM.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace GItClient.Core.Models
{
    public class UITab
    {
        public Border TabBorder { get; set; }
        public Grid TabGrid { get; set; }
        public TextBlock TabTextBlock { get; set; }
        public CrossButton TabButton { get; set; }


        private string _name;
        public string Name { get { return _name; } 
                            set {
                                    _name = value;  
                                    TabTextBlock.Text = value; } }

        public SolidColorBrush Background
        {
            get { return (SolidColorBrush)TabBorder.Background; }
            set
            {
                TabBorder.Background = value;
                Opacity = 0.6;
            }
        }

        public double Opacity
        {
            get { return TabBorder.Background.Opacity; }
            set
            {
                TabBorder.Background.Opacity = value;
            }
        }

        public MouseButtonEventHandler LeftButtonDown { get; set; }

        public RoutedEventHandler CloseClick { get; set; }

        public Grid Parent { get; set; }

        private ColumnDefinition Column { get; set; }

        public bool IsActive => Opacity == 1;

        public UITab()
        {
            TabBorder = new Border();
            TabGrid = new Grid();
            TabTextBlock = new TextBlock();
            TabButton = new CrossButton();

            TabButton.Width = 13;
            TabButton.Height = 13;
            TabButton.HorizontalAlignment = HorizontalAlignment.Right;
            TabButton.Margin = new Thickness(0, 0, 6, 0);

            TabTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            TabTextBlock.VerticalAlignment = VerticalAlignment.Center;

            TabGrid.ColumnDefinitions.Add(new ColumnDefinition());
            TabGrid.ColumnDefinitions.Add(new ColumnDefinition());

            TabBorder.Child = TabGrid;
            Background = new SolidColorBrush(Color.FromArgb(255, 49, 43, 64));

            TabGrid.Children.Add(TabTextBlock);
            TabGrid.Children.Add(TabButton);

            Grid.SetRow(TabTextBlock, 0);
            Grid.SetColumn(TabTextBlock, 0);
            Grid.SetColumnSpan(TabTextBlock, 2);

            Grid.SetRow(TabButton, 0);
            Grid.SetColumn(TabButton, 1);

            TabBorder.MouseLeftButtonDown += (object o, MouseButtonEventArgs e) => LeftButtonDown?.Invoke(this, e);
            TabButton.Click += (object o, RoutedEventArgs e) => CloseClick?.Invoke(this, e);
        }


        public void AddTabToParent(int index)
        {
            Column = new ColumnDefinition();
            Parent.ColumnDefinitions.Add(Column);
 
            Parent.Children.Add(this.TabBorder);
            Grid.SetRow(this.TabBorder, 0);
            Grid.SetColumn(this.TabBorder, index);
        }

        public void SetColumn(int index)
        {
            Grid.SetRow(this.TabBorder, 0);
            Grid.SetColumn(this.TabBorder, index);
        }

        public void Remove()
        {
            Parent.ColumnDefinitions.Remove(Column);
            Parent.Children.Remove(this.TabBorder);
        }

        public void Activate()
        {
            Opacity = 1;
        }
        public void Deactivate()
        {
            Opacity = 0.3;
        }

        public void ConvertToDefault()
        {
            Name = "Welcome!";
            TabButton.Visibility = Visibility.Hidden;
        }

    }
}
