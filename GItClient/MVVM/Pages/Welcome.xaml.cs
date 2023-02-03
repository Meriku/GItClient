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
using System.Net.Mail;
using GItClient.Core.Models;
using System.Threading.Tasks;

namespace GItClient.MVVM.Pages
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        private const int MAX_TEXT_LENGTH = 25;

        public RelayCommand MaximizedMinimizedWindow { get; set; }

        private string? UserName;
        private string? Email;
        private string? Directory;

        public Welcome()
        {
            InitializeComponent();

            var WelcomeViewModel = new WelcomeViewModel();
            Directory = WelcomeViewModel.DefaultDriveName;
            DataContext = WelcomeViewModel;

            MaximizedMinimizedWindow = new RelayCommand(headerControlBar_MouseLeftDoubleClick);
            headerBorder.InputBindings.Add(new InputBinding(MaximizedMinimizedWindow, new MouseGesture(MouseAction.LeftDoubleClick)));
        }

        private async void button_Finish_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.IsEnabled = false;

            UserName = User_Name_Box.Text;

            var userSettinsController = ControllersProvider.GetUserSettingsController();
            await Task.Run(async () =>
            {
                await userSettinsController.SetAndSaveUserSettings(UserName, Email, Directory);
            });

            var newWindow = new MainWindow();
            Application.Current.MainWindow = newWindow;
            newWindow.Show();
            this.Close();
        }

        private void textBoxEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            var email = Email_Box.Text;
            if (email.Length == 0) return;

            if (MailAddress.TryCreate(email, out var result))
            {
                Email = email;
                Email_Box.Text = result.Address;
                email_Error_Icon.Visibility = Visibility.Hidden;
            }
            else
            {
                email_Error_Icon.Visibility = Visibility.Visible;
            }

        }

        private void onclick_Open_Directory_Dialog(object sender, MouseButtonEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                Directory = dialog.FileName;
                ((TextBox)sender).Text = TrimDirectoryName(dialog.FileName);
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

        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        #region WindowControl
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
        #endregion
    }
}
