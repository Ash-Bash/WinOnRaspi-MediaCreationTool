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
    /// Interaction logic for SigningWindowsFilesPage.xaml
    /// </summary>
    public partial class SigningWindowsFilesPage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;

        public SigningWindowsFilesPage(MainWindow window, WinRaspItem raspItem)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;

            SignWindowsFiles();
        }

        private async void SignWindowsFiles() {

            var signUEFIFilesTask = Task.Run(() => {

                /*string[] bcdArgs = new string[3];
                bcdArgs[0] = "/c bcdboot i:\\windows /s p: /f UEFI && exit";

                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "bcdboot.exe");
                info.Arguments = "i:\\windows /s p: /f UEFI && exit";
                info.Verb = "runas";
                info.UseShellExecute = false;
                info.RedirectStandardOutput = true;

                Process cmd = new Process();
                cmd.EnableRaisingEvents = true;
                cmd.StartInfo = info;

                cmd.Start();

                Console.WriteLine("SignWindowsFiles() - BCDBoot Process: " + cmd.StandardOutput.ReadToEnd());

                cmd.WaitForExit();*/
                Process.Start(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "WinOnRaspi-Media-Creator-Tool", "temp", "InstallUEFI.cmd"));
            
            });

            await signUEFIFilesTask;

            var signWindowsFilesTask = Task.Run(() => {
                string[] bcdArgs = new string[3];
                bcdArgs[0] = "bcdedit /store " + @"p:\EFI\Microsoft\Boot\bcd" + " /set {default} testsigning on" + " && bcdedit /store " + @"p:\EFI\Microsoft\Boot\bcd" + " /set {default} nointegritychecks on && exit";

                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.Verb = "runas";

                //cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.EnableRaisingEvents = true;

                foreach (string arg in bcdArgs)
                {
                    cmd.StartInfo.Arguments += arg;
                }

                cmd.Start();

                Console.WriteLine(cmd.StandardOutput.ReadToEnd());

                cmd.WaitForExit();
            });

            await signWindowsFilesTask;

            /*var convertBootToEFITask = Task.Run(() =>
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "diskpart.exe");
                cmd.StartInfo.Verb = "runas";

                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.EnableRaisingEvents = true;

                cmd.Start();
                cmd.StandardInput.WriteLine("select disk " + raspItem.diskIndex);
                cmd.StandardInput.WriteLine("select partition 1");
                cmd.StandardInput.WriteLine("remove letter=P:");
                cmd.StandardInput.WriteLine("set id=C12A7328-F81F-11D2-BA4B-00A0C93EC93B");
                cmd.StandardInput.WriteLine("exit");

                Console.WriteLine("createSCCardPartitions() Process: " + cmd.StandardOutput.ReadToEnd());

                cmd.WaitForExit();
            });*/

            //await convertBootToEFITask;

            window.MainFrame.Content = new CleanUpPage(window, raspItem, true);
        }
    }
}
