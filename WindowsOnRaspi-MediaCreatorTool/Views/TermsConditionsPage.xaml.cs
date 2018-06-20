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

namespace WindowsOnRaspi_MediaCreatorTool.Views
{
    /// <summary>
    /// Interaction logic for TermsConditionsPage.xaml
    /// </summary>
    public partial class TermsConditionsPage : Page
    {
        // Variables
        private MainWindow window;

        public TermsConditionsPage(MainWindow window)
        {
            InitializeComponent();

            this.window = window;
            this.window.isLockdownMode = false;
            this.window.forceCleanUp = false;

            termsTextBox.Text = "Welcome to Windows on Raspberry Pi Imager! This program will image a modified version of Windows on ARM64 to an SD card for use with a Raspberry Pi 3B or 3B "+
                           "You will need:" + "" +
                           "- An SD card with at least 16GB capacity" +
                           "- A Raspberry Pi 3B or 3B+" +
                           "- Time and patience" + "" +
                           "Credits:" +
                           "andreiw for implementing the bootloader/UEFI for Raspberry Pi" +
                           "UEFI Github: https://github.com/andreiw/RaspberryPiPkg" + "" +
                           "Ash-Bash for the idea" +
                           "Github: https://github.com/Ash-Bash" + "" +
                           "WARNINGS:" +
                           "- Imaging Windows to your SD card will take at least an hour." +
                           "- This is NOT Windows 10 IOT. It's a complete copy of Windows 10 on ARM64 with a desktop and *.exe support." +
                           "- Drivers for the Raspberry Pi are still in development. Currently used USB driver isn't that great, it can randomly stop while using it. Currently used SD card driver is " +
                           "slow as high speed is not implemented, but it works with no issues. There are no drivers for anything other than USB and SD card." +
                           "- This tool is still in development as well. If you want to help developing this tool, be sure to check out the repository on Github.";
        }

        private void disagreeButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.GoBack();
        }

        private void agreeButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.Content = new SelectSDCardPage(window);
        }
    }
}
