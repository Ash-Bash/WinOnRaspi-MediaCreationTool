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
    /// Interaction logic for ManualSelectionDriversPage.xaml
    /// </summary>
    public partial class ManualSelectionDriversPage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;
        private string driverVersionName = "";

        public ManualSelectionDriversPage(MainWindow window, WinRaspItem raspItem)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;

            this.window.isLockdownMode = false;
            this.window.forceCleanUp = true;
            this.window.raspItem = raspItem;

            loadDriverPaths();
        }

        private void loadDriverPaths() {

            var directories = Directory.GetDirectories(System.IO.Path.Combine(raspItem.tempFolders[3], "RaspberryPiPkg", "Binary", "prebuilt"));

            foreach (string dir in directories) {
                driversListCombobox.Items.Add(new DirectoryInfo(dir).Name);
            }

            driversListCombobox.SelectedIndex = directories.Length - 3;
            driverVersionName = new DirectoryInfo(directories[directories.Length - 3]).Name;
        }

        private void driversListCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var directories = Directory.GetDirectories(System.IO.Path.Combine(raspItem.tempFolders[3], "RaspberryPiPkg", "Binary", "prebuilt"));

            driverVersionName = new DirectoryInfo(directories[comboBox.SelectedIndex]).Name;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This Action will stop the installation process and cleanup any files that was copied over, Do you want to Continue?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // user clicked yes
                window.MainFrame.Content = new CleanUpPage(window, raspItem, false);
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

        private async void nextButton_Click(object sender, RoutedEventArgs e)
        {
            string driversBinaryMode = "RELEASE";

            if ((bool)isDebugModeCheckbox.IsChecked)
            {
                driversBinaryMode = "DEBUG";
            }
            else {
                driversBinaryMode = "RELEASE";
            }

            var packagesTask = Task.Run(() =>
            {
                var UEFIPath = System.IO.Path.Combine(raspItem.tempFolders[3], "RaspberryPiPkg", "Binary", "prebuilt");
                var UEFIFilePaths = Directory.GetDirectories(UEFIPath);
                Debug.WriteLine("UEFI Files Paths: " + UEFIFilePaths);
                CopyFilesRecursively(new DirectoryInfo(System.IO.Path.Combine(UEFIPath, driverVersionName, driversBinaryMode)), new DirectoryInfo(raspItem.tempFolders[0]));
            });

            await packagesTask;

            window.MainFrame.Content = new ModifingInstallWimPage(window, raspItem); 
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
