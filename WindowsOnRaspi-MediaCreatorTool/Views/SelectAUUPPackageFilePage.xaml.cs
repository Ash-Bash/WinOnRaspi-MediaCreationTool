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
    /// Interaction logic for SelectAUUPPackageFilePage.xaml
    /// </summary>
    public partial class SelectAUUPPackageFilePage : Page
    {

        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;
        private dynamic langjson;

        public SelectAUUPPackageFilePage(MainWindow window, WinRaspItem raspItem, dynamic lang)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;
            this.langjson = lang;

            if (langjson != null && langjson.pages.selectAUUPPackageFilePage != null)
            {
                titleTextBlock.Text = langjson.pages.selectAUUPPackageFilePage.title;
                uupPackageTextBlock.Text = langjson.pages.selectAUUPPackageFilePage.uupPackageTextFieldLabel;

                cancelButton.Content = langjson.common_elements.cancel_button;
                backButton.Content = langjson.common_elements.back_button;
                nextButton.Content = langjson.common_elements.next_button;
            }

            this.window.isLockdownMode = false;
            this.window.forceCleanUp = false;
        }

        private void uupPackageButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog();
            openFileDialog.Filters.Add(new CommonFileDialogFilter("Compression File", "zip"));
            CommonFileDialogResult result = openFileDialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string dir = openFileDialog.FileName;
                raspItem.uupPackagePath = dir;
                uupPackageTextBox.Text = dir;
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
            window.MainFrame.GoBack();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {

            string emptyPathAlertTitle = langjson.alerts_messages.emptyPathAlert.title;
            string emptyPathMessage = langjson.alerts_messages.emptyPathAlert.message;

            if (raspItem.uupPackagePath != null)
            {
                window.MainFrame.Content = new DownloadingWindowsISOFilePage(window, raspItem, langjson);
            }
            else
            {
                MessageBox.Show(emptyPathMessage, emptyPathAlertTitle);
            }
        }
    }
}
