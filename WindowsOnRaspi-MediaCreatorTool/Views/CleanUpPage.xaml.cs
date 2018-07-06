using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WindowsOnRaspi_MediaCreatorTool.Classes;

namespace WindowsOnRaspi_MediaCreatorTool.Views
{
    /// <summary>
    /// Interaction logic for CleanUpPage.xaml
    /// </summary>
    public partial class CleanUpPage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;
        private bool wasSuccess;
        private bool willClose;
        private dynamic langjson;

        public CleanUpPage(MainWindow window, WinRaspItem raspItem, dynamic lang, bool wasSuccess, bool willClose = false)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;
            this.wasSuccess = wasSuccess;
            this.willClose = willClose;
            this.langjson = lang;

            if (langjson != null && langjson.pages.cleanUpPage != null)
            {
                titleTextBlock.Text = langjson.pages.cleanUpPage.title;
                subtitleTextBlock.Text = langjson.pages.cleanUpPage.subtitle;
            }

            this.window.isLockdownMode = true;
            this.window.forceCleanUp = false;

            cleanUp();
        }

        private async void cleanUp() {

            var unmountWimTask = Task.Run(() => {
                string[] dismArgs = new string[3];
                dismArgs[0] = "/cleanup-wim";

                Process cmd = new Process();
                cmd.StartInfo.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "dism.exe");
                cmd.StartInfo.Verb = "runas";

                foreach (string arg in dismArgs)
                {
                    cmd.StartInfo.Arguments += arg;
                }

                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.EnableRaisingEvents = true;

                cmd.Start();

                Console.WriteLine("unmountInstallWimFile() Process: " + cmd.StandardOutput.ReadToEnd());

                cmd.WaitForExit();
            });

            await unmountWimTask;


            var deleteFilesTask = Task.Run(() =>
            {
                try
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(System.IO.Path.Combine(raspItem.appFolderPath, "temp"));

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                }
                catch {
                }
            });

            await deleteFilesTask;

            if (willClose)
            {
                window.MainFrame.Content = new CompletedSetupPage(window, raspItem, wasSuccess, willClose);
            }
            else
            {
                window.MainFrame.Content = new CompletedSetupPage(window, raspItem, langjson, wasSuccess);
            }
        }
    }
}
