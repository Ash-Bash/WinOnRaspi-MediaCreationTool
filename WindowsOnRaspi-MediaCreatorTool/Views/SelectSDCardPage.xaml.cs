using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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
    /// Interaction logic for SelectSDCardPage.xaml
    /// </summary>
    public partial class SelectSDCardPage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;
        private string[] sdSizes;

        private int[] diskIndexList = { 0 };

        public SelectSDCardPage(MainWindow window)
        {
            InitializeComponent();

            this.window = window;
            raspItem = new WinRaspItem();

            this.window.isLockdownMode = false;
            this.window.forceCleanUp = false;

            RefreshDisks();
        }

        private void RefreshDisks()
        {
            sdCardCombobox.Items.Clear();
            diskIndexList = null;
            sdSizes = null;
            ManagementObjectSearcher win32DiskDrives = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            diskIndexList = new int[win32DiskDrives.Get().Count];
            sdSizes = new string[win32DiskDrives.Get().Count];
    
            int i = 0;
            foreach (ManagementObject win32DiskDrive in win32DiskDrives.Get())
            {
                Int64 size = new Int64();
                int index = Convert.ToInt32(win32DiskDrive.Properties["Index"].Value);
                string model = win32DiskDrive.Properties["Model"].Value.ToString();
                string mediaType;
                if (win32DiskDrive.Properties["Size"].Value != null)
                {
                    string sizeString = win32DiskDrive.Properties["Size"].Value.ToString();
                    size = Int64.Parse(sizeString) / 1024 / 1024 / 1024;
                }
                else
                {
                    size = 0;
                }

                sdSizes[i] = size.ToString();

                if (win32DiskDrive.Properties["MediaType"].Value != null)
                {
                    mediaType = win32DiskDrive.Properties["MediaType"].Value.ToString();
                }
                else
                {
                    mediaType = "Unknown Media Type";
                }
                diskIndexList[i] = index;
                i++;
                sdCardCombobox.Items.Add("Disk " + index + " - " + model + " - " + mediaType + " - " + size.ToString() + "GB");
            }

            if (sdCardCombobox.Items.Count > 0)
            {
                sdCardCombobox.SelectedIndex = 0;
                raspItem.diskIndex = diskIndexList[sdCardCombobox.SelectedIndex];
            }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshDisks();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.GoBack();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            raspItem.diskIndex = diskIndexList[sdCardCombobox.SelectedIndex];
            raspItem.sdSize = sdSizes[sdCardCombobox.SelectedIndex];

            int sdSize = int.Parse(raspItem.sdSize);
            if (sdSize >= 14 && sdSize < 29 ) {
                if (MessageBox.Show("It is Recommend that you use a 32GB or Higher Drive, Do you want to Continue?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    // user clicked yes
                    if (MessageBox.Show("This Action will Format and Erase all data that was on the drive you selected, Do you want to Continue?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        // user clicked yes
                        window.MainFrame.Content = new FormatSDCardPage(window, raspItem);
                    }
                    else
                    {
                        // user clicked no
                    }
                }
                else
                {
                    // user clicked no
                }
            } else if (sdSize >= 29)
            {
                if (MessageBox.Show("This Action will Format and Erase all data that was on the drive you selected, Do you want to Continue?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    // user clicked yes
                    window.MainFrame.Content = new FormatSDCardPage(window, raspItem);
                }
                else
                {
                    // user clicked no
                }

            } else
            {
                MessageBox.Show("Please uses a Drive that is 16GBs or Higher!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This Action will stop the installation process, Do you want to Continue?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // user clicked yes
                window.MainFrame.Content = new CompletedSetupPage(window, raspItem, false);
            }
            else
            {
                // user clicked no
            }
        }

        private void sdCardCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            /*if (diskIndexList != null && sdSizes != null)
            {
                if (diskIndexList.Length > 0 && sdSizes.Length > 0)
                {
                    raspItem.diskIndex = diskIndexList[comboBox.SelectedIndex];
                    raspItem.sdSize = sdSizes[comboBox.SelectedIndex];
                }
            }*/
        }
    }
}
