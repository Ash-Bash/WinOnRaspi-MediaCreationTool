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

        public CleanUpPage(MainWindow window, WinRaspItem raspItem, bool wasSuccess)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;
            this.wasSuccess = wasSuccess;

            cleanUp();
        }

        private async void cleanUp() {
            DirectoryInfo extrFolders = new DirectoryInfo(raspItem.tempFolders[1]);
            FileInfo[] fileInfo = extrFolders.GetFiles();

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
                System.IO.DirectoryInfo di = new DirectoryInfo(System.IO.Path.Combine(raspItem.appFolderPath, "temp"));

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            });

            await deleteFilesTask;

            window.MainFrame.Content = new CompletedSetupPage(window, raspItem, wasSuccess);
        }
    }
}
