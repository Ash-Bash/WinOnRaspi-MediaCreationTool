using Microsoft.WindowsAPICodePack.Dialogs;
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
using WindowsOnRaspi_MediaCreatorTool.Classes;

namespace WindowsOnRaspi_MediaCreatorTool.Views
{
    /// <summary>
    /// Interaction logic for SelectPackagesPage.xaml
    /// </summary>
    public partial class SelectPackagesPage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;

        private dynamic langjson;

        public SelectPackagesPage(MainWindow window, WinRaspItem raspItem, dynamic lang)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;
            this.langjson = lang;

            if (langjson != null)
            {
                titleTextBlock.Text = langjson.pages.selectingRaspiPackagesPage.title;
                raspberryPiPkgTextBlock.Text = langjson.pages.selectingRaspiPackagesPage.raspPiPkgTextFieldLabel;
                windowsOnRaspiTextBlock.Text = langjson.pages.selectingRaspiPackagesPage.winOnRaspTextFieldLabel;

                cancelButton.Content = langjson.common_elements.cancel_button;
                nextButton.Content = langjson.common_elements.next_button;
            }

            this.window.isLockdownMode = false;
            this.window.forceCleanUp = false;
        }

        private void browseRaspiPkgButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog();
            openFileDialog.Filters.Add(new CommonFileDialogFilter("Compression File", "zip"));
 
            CommonFileDialogResult result = openFileDialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string dir = openFileDialog.FileName;
                raspItem.raspPiPkgPath = dir;
                browseRaspiPkgTextBox.Text = dir;
            }
        }

        private void browseWindowsOnRaspiButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog();
            openFileDialog.Filters.Add(new CommonFileDialogFilter("Compression File", "zip"));
            CommonFileDialogResult result = openFileDialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string dir = openFileDialog.FileName;
                raspItem.rpiwinStuffPath = dir;
                windowsOnRaspiTextBox.Text = dir;
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {

            string cancelAlertTitle = langjson.alerts_messages.cancelAlert.title;
            string cancelAlertMessage = langjson.alerts_messages.cancelAlert.message;

            if (MessageBox.Show(cancelAlertMessage, cancelAlertTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // user clicked yes
                window.MainFrame.Content = new CompletedSetupPage(window, raspItem, langjson, false);
            }
            else
            {
                // user clicked no
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {

            string oneOrMorePathsEmptyAlertTitle = langjson.alerts_messages.oneOrMorePathsEmptyAlert.title;
            string oneOrMorePathsEmptyMessage = langjson.alerts_messages.oneOrMorePathsEmptyAlert.message;

            if (raspItem.raspPiPkgPath != null && raspItem.rpiwinStuffPath != null)
            {
                window.MainFrame.Content = new ISOFiIeSourceOptionsPage(window, raspItem, langjson);
            } else
            {
                MessageBox.Show(oneOrMorePathsEmptyMessage, oneOrMorePathsEmptyAlertTitle);
            }
        }
    }
}
