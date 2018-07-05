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
        private dynamic langjson;

        public TermsConditionsPage(MainWindow window, dynamic lang)
        {
            InitializeComponent();

            this.window = window;
            this.window.isLockdownMode = false;
            this.window.forceCleanUp = false;
            this.langjson = lang;

            if (langjson != null)
            {
                titleTextBlock.Text = langjson.pages.termsPage.title;

                string apppath = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
                string rtfpath = apppath + langjson.pages.termsPage.rtfFileLink;
                using (StreamReader r = new StreamReader(rtfpath))
                {
                    byte[] byteArray = Encoding.ASCII.GetBytes(r.ReadToEnd());
                    using (var reader = new MemoryStream(byteArray))
                    {
                        termsTextBox.Selection.Load(reader, DataFormats.Rtf);
                    }
                }


                disagreeButton.Content = langjson.common_elements.disagree_button;
                agreeButton.Content = langjson.common_elements.agree_button;
            }
            else {
                byte[] byteArray = Encoding.ASCII.GetBytes(Properties.Resources.WinOnRaspiMediaCreationToolTerms);
                using (var reader = new MemoryStream(byteArray))
                {
                    termsTextBox.Selection.Load(reader, DataFormats.Rtf);
                }
            }
        }

        private void disagreeButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.GoBack();
        }

        private void agreeButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.Content = new SelectSDCardPage(window, langjson);
        }
    }
}
