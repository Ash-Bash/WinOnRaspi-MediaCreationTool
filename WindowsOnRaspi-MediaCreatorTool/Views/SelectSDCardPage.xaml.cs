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

        private int[] diskIndexList = { 0 };

        public SelectSDCardPage(MainWindow window)
        {
            InitializeComponent();

            this.window = window;
            raspItem = new WinRaspItem();

            RefreshDisks();
        }

        private void RefreshDisks()
        {
            sdCardCombobox.Items.Clear();
            ManagementObjectSearcher win32DiskDrives = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            diskIndexList = new int[win32DiskDrives.Get().Count];
            int i = 0;
            foreach (ManagementObject win32DiskDrive in win32DiskDrives.Get())
            {
                Int64 size;
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
            window.MainFrame.Content = new FormatSDCardPage(window, raspItem);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.Content = new CompletedSetupPage(window, raspItem, false);
        }

        private void sdCardCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            raspItem.diskIndex = diskIndexList[comboBox.SelectedIndex];
        }
    }
}
