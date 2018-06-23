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

        public HasSDFormatedProperlyPage(MainWindow window, WinRaspItem raspItem)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;

            this.window.isLockdownMode = false;
            this.window.forceCleanUp = false;
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

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.Content = new FormatSDCardPage(window, raspItem);
        }

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.Content = new SelectPackagesPage(window, raspItem);
        }
    }
}
