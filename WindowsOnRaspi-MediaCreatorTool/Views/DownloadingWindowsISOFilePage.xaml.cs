using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
    /// Interaction logic for DownloadingWindowsISOFilePage.xaml
    /// </summary>
    public partial class DownloadingWindowsISOFilePage : Page
    {

        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;
        private dynamic langjson;

        public DownloadingWindowsISOFilePage(MainWindow window, WinRaspItem raspItem, dynamic lang)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;
            this.langjson = lang;

            if (langjson != null && langjson.pages.downloadingWindowsISOFilePage != null)
            {
                titleTextBlock.Text = langjson.pages.downloadingWindowsISOFilePage.title;
                subtitleTextBlock.Text = langjson.pages.downloadingWindowsISOFilePage.subtitle;
            }

            this.window.isLockdownMode = true;
            this.window.forceCleanUp = false;

            DownloadWindows();
        }

        private async void DownloadWindows() {
            var tempUUPPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "WinOnRaspi-Media-Creator-Tool", "temp", "UUP");
            var createUUPISOpath = System.IO.Path.Combine(tempUUPPath, "creatingISO.cmd");

            var createDirTask = Task.Run(() =>
            {
                // Creates Directories for Required Folders+
                Directory.CreateDirectory(tempUUPPath);

                ZipFile.ExtractToDirectory(raspItem.uupPackagePath, tempUUPPath);
            });

            await createDirTask;

            var createISOTask = Task.Run(() =>
            {
                Debug.WriteLine("ISO CMD Path Exists: " + true);

                /*Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                cmd.StandardInput.WriteLine(createUUPISOpath);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();*/

                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.Verb = "runas";

                cmd.StartInfo.CreateNoWindow = false;
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.EnableRaisingEvents = true;

                cmd.Start();
                cmd.StandardInput.WriteLine(createUUPISOpath);
                Console.WriteLine("createWinISO() Process: " + cmd.StandardOutput.ReadToEnd());

                cmd.WaitForExit();
            });

            await createISOTask;

            var getISOTask = Task.Run(() =>
            {
                string[] isofiles = System.IO.Directory.GetFiles(tempUUPPath, "*.iso");

                Debug.WriteLine("ISO File Path: " + isofiles[0]);
                raspItem.winImagePath = isofiles[0];
            });

            await getISOTask;

            if (raspItem.winImagePath != null)
            {
                window.MainFrame.Content = new SettingUpTempFilesPage(window, raspItem, langjson);
            }
            else {
                window.MainFrame.Content = new CleanUpPage(window, raspItem, langjson, false);
            }

        }
    }
}
