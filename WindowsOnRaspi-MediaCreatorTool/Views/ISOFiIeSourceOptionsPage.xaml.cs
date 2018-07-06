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
    /// Interaction logic for ISOFiIeSourceOptionsPage.xaml
    /// </summary>
    public partial class ISOFiIeSourceOptionsPage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;
        private bool isWindowsPath = true;
        private dynamic langjson;

        public ISOFiIeSourceOptionsPage(MainWindow window, WinRaspItem raspItem, dynamic lang)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;
            this.langjson = lang;

            if (langjson != null && langjson.pages.isoSelectionOptionsPage != null)
            {
                titleTextBlock.Text = langjson.pages.isoSelectionOptionsPage.title;

                selectAWindowsISOFilePathRadioButton.Content = langjson.pages.isoSelectionOptionsPage.option_1;
                downloadUsingUUPMethodRadioButton.Content = langjson.pages.isoSelectionOptionsPage.option_2;

                cancelButton.Content = langjson.common_elements.cancel_button;
                backButton.Content = langjson.common_elements.back_button;
                nextButton.Content = langjson.common_elements.next_button;
            }

            this.window.isLockdownMode = false;
            this.window.forceCleanUp = false;
            this.window.raspItem = raspItem;
        }

        private void ISOFileOptionsRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;

            switch (radioButton.Tag)
            {
                case "isopath":
                    isWindowsPath = true;
                    break;
                case "uupmethod":
                    isWindowsPath = false;
                    break;
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            string cancelAlertTitle = langjson.alerts_messages.cancelAlert.title;
            string cancelAlertMessage = langjson.alerts_messages.cancelAlert.message;

            if (MessageBox.Show(cancelAlertMessage, cancelAlertTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
            window.MainFrame.GoBack();
        }

        private async void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (isWindowsPath)
            {
                window.MainFrame.Content = new SelectWindowsISOFilePage(window, raspItem, langjson);
            }
            else
            {
                window.MainFrame.Content = new SelectAUUPPackageFilePage(window, raspItem, langjson);
            }
        }
    }
}
