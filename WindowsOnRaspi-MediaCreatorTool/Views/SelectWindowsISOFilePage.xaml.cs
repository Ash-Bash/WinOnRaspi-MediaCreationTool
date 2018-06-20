﻿using Microsoft.WindowsAPICodePack.Dialogs;
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
    /// Interaction logic for SelectWindowsISOFilePage.xaml
    /// </summary>
    public partial class SelectWindowsISOFilePage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;

        public SelectWindowsISOFilePage(MainWindow window, WinRaspItem raspItem)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;

            this.window.isLockdownMode = false;
            this.window.forceCleanUp = false;
        }

        private void winImageButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog();
            openFileDialog.Filters.Add(new CommonFileDialogFilter("Disk Image", "iso"));
            CommonFileDialogResult result = openFileDialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string dir = openFileDialog.FileName;
                raspItem.winImagePath = dir;
                winImageTextBox.Text = dir;
            }
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

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.GoBack();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (raspItem.winImagePath != null)
            {
                window.MainFrame.Content = new SettingUpTempFilesPage(window, raspItem);
            }
            else
            {
                MessageBox.Show("Can't continue because you haven't provided a path to a valid .iso file", "Error!");
            }
        }
    }
}
