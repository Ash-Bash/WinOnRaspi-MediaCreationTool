using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for SelectedRecommendedDriversPage.xaml
    /// </summary>
    public partial class SelectedRecommendedDriversPage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;
        private bool isAutoDrivers = true;
        private dynamic langjson;

        public SelectedRecommendedDriversPage(MainWindow window, WinRaspItem raspItem, dynamic lang)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;
            this.langjson = lang;

            if (langjson != null && langjson.pages.selectedRecommendedDriversPage != null)
            {
                titleTextBlock.Text = langjson.pages.selectedRecommendedDriversPage.title;

                useRecommendDriversRadioButton.Content = langjson.pages.selectedRecommendedDriversPage.option_1;
                manuallySelectDriversRadioButton.Content = langjson.pages.selectedRecommendedDriversPage.option_2;

                cancelButton.Content = langjson.common_elements.cancel_button;
                nextButton.Content = langjson.common_elements.next_button;
            }

            this.window.isLockdownMode = false;
            this.window.forceCleanUp = true;
            this.window.raspItem = raspItem;
        }

        private void DriversRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;

            switch (radioButton.Tag)
            {
                case "auto":
                    isAutoDrivers = true;
                    break;
                case "manual":
                    isAutoDrivers = false;
                    break;
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {

            string cancelAfterCleanUpAlertTitle = langjson.alerts_messages.cancelAfterCleanUpAlert.title;
            string cancelAfterCleanUpAlertMessage = langjson.alerts_messages.cancelAfterCleanUpAlert.message;

            if (MessageBox.Show(cancelAfterCleanUpAlertMessage, cancelAfterCleanUpAlertTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // user clicked yes
                window.MainFrame.Content = new CleanUpPage(window, raspItem, langjson, false);
            }
            else
            {
                // user clicked no
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            //window.MainFrame.GoBack();
        }

        private async void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (isAutoDrivers) {
                var packagesTask = Task.Run(() =>
                {
                    var UEFIPath = System.IO.Path.Combine(raspItem.tempFolders[3], "RaspberryPiPkg", "Binary", "prebuilt");
                    var UEFIFilePaths = Directory.GetDirectories(UEFIPath);
                    Debug.WriteLine("UEFI Files Paths: " + UEFIFilePaths);
                    CopyFilesRecursively(new DirectoryInfo(System.IO.Path.Combine(UEFIFilePaths[UEFIFilePaths.Length - 3], "DEBUG")), new DirectoryInfo(raspItem.tempFolders[0]));
                });

                await packagesTask;

                window.MainFrame.Content = new ModifingInstallWimPage(window, raspItem, langjson);
            }
            else {
                window.MainFrame.Content = new ManualSelectionDriversPage(window, raspItem, langjson);
            }
        }

        // Allows Copying files and Folders from one Dir to Another
        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                foreach (DirectoryInfo dir in source.GetDirectories())
                    CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
                foreach (FileInfo file in source.GetFiles())
                    file.CopyTo(System.IO.Path.Combine(target.FullName, file.Name));
            }
            catch (Exception e)
            {
                Debug.WriteLine("Somewthing Went Wrong!! " + e);
            }
        }
    }
}
