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
    /// Interaction logic for HasSDFormatedProperlyPage.xaml
    /// </summary>
    public partial class HasSDFormatedProperlyPage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;
        private dynamic langjson;

        public HasSDFormatedProperlyPage(MainWindow window, WinRaspItem raspItem, dynamic lang)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;
            this.langjson = lang;

            if (langjson != null)
            {
                titleTextBlock.Text = langjson.pages.hasSDFormatedProperlyPage.title;
                subtitle_1TextBlock.Text = langjson.pages.hasSDFormatedProperlyPage.subtitle_1;
                subtitle_2TextBlock.Text = langjson.pages.hasSDFormatedProperlyPage.subtitle_2;
                subtitle_3TextBlock.Text = langjson.pages.hasSDFormatedProperlyPage.subtitle_3;

                cancelButton.Content = langjson.common_elements.cancel_button;
                noButton.Content = langjson.common_elements.no_button;
                yesButton.Content = langjson.common_elements.yes_button;
            }

            this.window.isLockdownMode = false;
            this.window.forceCleanUp = false;
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

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.Content = new FormatSDCardPage(window, raspItem, langjson);
        }

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.Content = new SelectPackagesPage(window, raspItem, langjson);
        }
    }
}
