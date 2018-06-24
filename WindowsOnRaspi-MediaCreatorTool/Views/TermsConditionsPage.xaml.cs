using System;
using System.Collections.Generic;
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

            byte[] byteArray = Encoding.ASCII.GetBytes(Properties.Resources.WinOnRaspiMediaCreationToolTerms);
            using (var reader = new MemoryStream(byteArray))
            {
                termsTextBox.Selection.Load(reader, DataFormats.Rtf);
            }
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
