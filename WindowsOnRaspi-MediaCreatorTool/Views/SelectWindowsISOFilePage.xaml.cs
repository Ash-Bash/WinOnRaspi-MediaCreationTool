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
    /// Interaction logic for SelectWindowsISOFilePage.xaml
    /// </summary>
    public partial class SelectWindowsISOFilePage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;
        private dynamic langjson;

        public SelectWindowsISOFilePage(MainWindow window, WinRaspItem raspItem, dynamic lang)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;
            this.langjson = lang;

            if (langjson != null && langjson.pages.selectWindowsISOFilePage != null)
            {
                titleTextBlock.Text = langjson.pages.selectWindowsISOFilePage.title;
                winImageTextBlock.Text = langjson.pages.selectWindowsISOFilePage.winImageTextFieldLabel;

                cancelButton.Content = langjson.common_elements.cancel_button;
                backButton.Content = langjson.common_elements.back_button;
                nextButton.Content = langjson.common_elements.next_button;
            }

            this.window.isLockdownMode = false;
            this.window.forceCleanUp = false;
        }

        private void winImageButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog();
            openFileDialog.Filters.Add(new CommonFileDialogFilter("Disk Image", "iso"));
            CommonFileDialogResult result = openFileDialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string dir = openFileDialog.FileName;
                raspItem.winImagePath = dir;
                winImageTextBox.Text = dir;
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

            string invalidISOFileAlertTitle = langjson.alerts_messages.invalidISOFileAlert.title;
            string invalidISOFileMessage = langjson.alerts_messages.invalidISOFileAlert.message;

            if (raspItem.winImagePath != null)
            {
                window.MainFrame.Content = new SettingUpTempFilesPage(window, raspItem, langjson);
            }
            else
            {
                MessageBox.Show(invalidISOFileMessage, invalidISOFileAlertTitle);
            }
        }
    }
}
