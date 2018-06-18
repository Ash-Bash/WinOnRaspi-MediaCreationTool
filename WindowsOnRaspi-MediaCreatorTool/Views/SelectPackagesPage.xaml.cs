using Microsoft.WindowsAPICodePack.Dialogs;
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
    /// Interaction logic for SelectPackagesPage.xaml
    /// </summary>
    public partial class SelectPackagesPage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;

        public SelectPackagesPage(MainWindow window, WinRaspItem raspItem)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;
        }

        private void browseRaspiPkgButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog();
            CommonFileDialogResult result = openFileDialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string dir = openFileDialog.FileName;
                raspItem.raspPiPkgPath = dir;
                browseRaspiPkgTextBox.Text = dir;
            }
        }

        private void browseWindowsOnRaspiButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog();
            CommonFileDialogResult result = openFileDialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string dir = openFileDialog.FileName;
                raspItem.rpiwinStuffPath = dir;
                windowsOnRaspiTextBox.Text = dir;
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.Content = new CompletedSetupPage(window, raspItem, false);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.Content = new SelectWindowsISOFilePage(window, raspItem);
        }
    }
}
