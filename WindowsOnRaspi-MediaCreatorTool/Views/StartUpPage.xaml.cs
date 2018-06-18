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
    /// Interaction logic for StartUpPage.xaml
    /// </summary>
    public partial class StartUpPage : Page
    {

        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;

        public StartUpPage(MainWindow window)
        {
            InitializeComponent();
            this.window = window;
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.Content = new TermsConditionsPage(window);
            window.MainFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        }
    }
}
