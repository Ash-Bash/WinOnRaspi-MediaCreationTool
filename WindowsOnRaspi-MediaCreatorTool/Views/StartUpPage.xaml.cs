using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for StartUpPage.xaml
    /// </summary>
    public partial class StartUpPage : Page
    {

        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;
        private dynamic langjson;

        public StartUpPage(MainWindow window, dynamic lang)
        {
            InitializeComponent();
            this.window = window;
            this.window.isLockdownMode = false;
            this.window.forceCleanUp = false;
            this.langjson = lang;

            if (langjson != null && langjson.pages.welcomePage != null) {
                titleTextBlock.Text = langjson.pages.welcomePage.title;
                titleSecondaryTextBlock.Text = langjson.pages.welcomePage.title_secondary;
                subtitleTextBlock.Text = langjson.pages.welcomePage.subtitle;
                disclaimerTextBlock.Text = langjson.pages.welcomePage.disclaimer;

                startButton.Content = langjson.common_elements.start_button;
            }

            Debug.WriteLine("Windows OS Version: " + Environment.OSVersion.Version.ToString());
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.Content = new TermsConditionsPage(window, langjson);
            //window.MainFrame.Content = new SigningWindowsFilesPage(window, null);
            window.MainFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        }
    }
}
