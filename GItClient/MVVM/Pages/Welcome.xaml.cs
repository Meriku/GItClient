using GItClient.Core;
using GItClient.MVVM.ViewModel;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text;
using GItClient.Core.Controllers;

namespace GItClient.MVVM.Pages
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        public RelayCommand MaximizedMinimizedWindow { get; set; }

        private UserSettingsController _userSettingsController { get; set; }
        private string _userDirectory { get; set; }

        private const int MAX_TEXT_LENGTH = 25;

        public Welcome()
        {
            InitializeComponent();

            MaximizedMinimizedWindow = new RelayCommand(headerControlBar_MouseLeftDoubleClick);
            headerBorder.InputBindings.Add(new InputBinding(MaximizedMinimizedWindow, new MouseGesture(MouseAction.LeftDoubleClick)));

            DataContext = new WelcomeViewModel();

            _userSettingsController = new UserSettingsController();
        }

        //TODO: Validate email, set default directory if empty
        private void button_Finish_Click(object sender, RoutedEventArgs e)
        {
            _userSettingsController.SetUserSettings(User_Name_Box.Text, Email_Box.Text, _userDirectory);

            var newWindow = new MainWindow();
            Application.Current.MainWindow = newWindow;
            newWindow.Show();
            this.Close();
        }

        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void onclick_Open_Directory_Dialog(object sender, MouseButtonEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                _userDirectory = TrimDirectoryName(dialog.FileName);
                ((TextBox)sender).Text = _userDirectory;
            }   
        }

        private string TrimDirectoryName(string input)
        {
            if (input.Length <= MAX_TEXT_LENGTH)
            {
                return input;
            }
            var folders = input.Split('\\');
            var result = new StringBuilder();
            foreach(var folder in folders)
            {
                if (result.Length + folder.Length > MAX_TEXT_LENGTH)
                {
                    return result + "...";
                }
                result.Append(folder + "\\");
            }
            return result.ToString();
        }


        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void headerControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void headerControlBar_MouseLeftDoubleClick(object commandParameter)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
            else
                this.WindowState = System.Windows.WindowState.Maximized;
        }
    }
}
