using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WindowsOnRaspi_MediaCreatorTool.Classes;

namespace WindowsOnRaspi_MediaCreatorTool.Views
{
    /// <summary>
    /// Interaction logic for CompletedSetupPage.xaml
    /// </summary>
    public partial class CompletedSetupPage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;
        private bool wasSuccess;
        private bool willClose;

        public CompletedSetupPage(MainWindow window, WinRaspItem raspItem, bool wasSuccess, bool willClose = false)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;
            this.wasSuccess = wasSuccess;

            this.window.isLockdownMode = false;
            this.window.forceCleanUp = false;

            if (wasSuccess)
            {
                titleTextBlock.Text = "Successfully Completed Windows Setup";
                SuccessStack.Visibility = Visibility.Visible;

                installUEFIButton.Content = raspItem.appFolderPath + "/SignWindows.cmd";
                //signUEFIButton.Content = raspItem.appFolderPath + "/SignUEFIFiles.cmd";
            }
            else {
                titleTextBlock.Text = "Unsuccessfully Completed Windows Setup";
                SuccessStack.Visibility = Visibility.Collapsed;
            }

            if (willClose) {
                window.Close();
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            window.Close();
        }

        private void installUEFIButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", raspItem.appFolderPath);
        }

        private void signUEFIButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", raspItem.appFolderPath);
        }
    }
}
