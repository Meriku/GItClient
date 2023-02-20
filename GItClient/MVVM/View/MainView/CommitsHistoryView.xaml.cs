using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GItClient.MVVM.View.MainView
{
    /// <summary>
    /// Interaction logic for CommitsHistoryView.xaml
    /// </summary>
    public partial class CommitsHistoryView : UserControl
    {
        private int ActiveRepos = 0;

        public CommitsHistoryView()
        {

            InitializeComponent();



        }

        private void UpdateMainGrid()
        {
            for (var i = 0; i < MainGrid.ColumnDefinitions.Count; i++)
            {
                if (i < ActiveRepos)
                {
                    MainGrid.ColumnDefinitions[i].MinWidth = 50;
                    MainGrid.ColumnDefinitions[i].Width = new GridLength(0, GridUnitType.Auto);
                }
                else
                {
                    MainGrid.ColumnDefinitions[i].MinWidth = 0;
                    MainGrid.ColumnDefinitions[i].Width = new GridLength(0, GridUnitType.Pixel);
                }
            }
        }

        private void Button_AddRepos_Click(object sender, MouseButtonEventArgs e)
        {
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition());


            Random random = new Random();     
            var color = Color.FromRgb( (byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255) );

            var border = new Border();
            border.Background = new SolidColorBrush(color);

            MainGrid.Children.Add(border);
            Grid.SetRow(border, 0);
            Grid.SetColumn(border, ActiveRepos);

            ActiveRepos++;
        }

        private void Button_RemoveRepos_Click(object sender, MouseButtonEventArgs e)
        {
            if (MainGrid.ColumnDefinitions.Any())
            {
                MainGrid.ColumnDefinitions.Remove(MainGrid.ColumnDefinitions.Last());
                ActiveRepos--;
            }
        }

    }
}
