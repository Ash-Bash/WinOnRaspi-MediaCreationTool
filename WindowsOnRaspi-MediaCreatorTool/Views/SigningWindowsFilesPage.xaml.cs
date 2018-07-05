using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        private dynamic langjson;

        public SigningWindowsFilesPage(MainWindow window, WinRaspItem raspItem, dynamic lang)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;

            this.window.isLockdownMode = true;
            this.window.forceCleanUp = true;
            this.window.raspItem = raspItem;
            this.langjson = lang;

            if (langjson != null)
            {
                titleTextBlock.Text = langjson.pages.signingWindowsFilesPage.title;
                subtitleTextBlock.Text = langjson.pages.signingWindowsFilesPage.subtitle;
            }

            SignWindowsFiles();
        }

        private async void SignWindowsFiles() {

            var signUEFIFilesTask = Task.Run(() => {

                string[] bcdArgs = new string[3];
                bcdArgs[0] = "/c bcdboot i:\\windows /s p: /f UEFI && exit";

                ProcessStartInfo info = new ProcessStartInfo();
                if (Environment.Is64BitOperatingSystem)
                {
                    info.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SysWOW64", "cmd.exe");
                }
                else
                {
                    info.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "cmd.exe");
                }
                info.WorkingDirectory = "C:\\Windows\\System32";
                info.Arguments = "/c bcdboot i:\\windows /s p: /f UEFI ";
                info.Verb = "runas";
                info.UseShellExecute = false;
                info.RedirectStandardOutput = true;

                Process cmd = new Process();
                cmd.EnableRaisingEvents = true;
                cmd.StartInfo = info;

                cmd.Start();

                Console.WriteLine("SignWindowsFiles() - BCDBoot Process: " + cmd.StandardOutput.ReadToEnd());

                cmd.WaitForExit();
                //Process.Start(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "WinOnRaspi-Media-Creator-Tool", "temp", "InstallUEFI.cmd"));

                /*string[] bcdArgs = new string[3];
                bcdArgs[0] = "/c"; // cmd.exe args
                bcdArgs[1] = "bcdboot " + @"i:\Windows /s p: /f UEFI"; // bcdboot.exe

                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.Verb = "runas";
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                cmd.StartInfo.UseShellExecute = true;
                cmd.StartInfo.RedirectStandardOutput = false;
                cmd.EnableRaisingEvents = true;

                foreach (string arg in bcdArgs)
                {
                    cmd.StartInfo.Arguments += arg + " "; // Adds a space after every argument
                }

                cmd.Start();

                //Console.WriteLine(cmd.StandardOutput.ReadToEnd());

                cmd.WaitForExit();*/

                //ExecuteCommandAsync("bcdboot i:\\Windows /s p: /f UEFI");


            });

            await signUEFIFilesTask;

            var signWindowsFilesTask = Task.Run(() => {
                string[] bcdArgs = new string[3];
                bcdArgs[0] = "/c";
                bcdArgs[1] = "bcdedit /store " + @"p:\EFI\Microsoft\Boot\bcd" + " /set {default} testsigning on" + " && bcdedit /store " + @"p:\EFI\Microsoft\Boot\bcd" + " /set {default} nointegritychecks on && exit";

                Process cmd = new Process();
                if (Environment.Is64BitOperatingSystem)
                {
                    cmd.StartInfo.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SysWOW64", "cmd.exe");
                }
                else
                {
                    cmd.StartInfo.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "cmd.exe");
                }
                cmd.StartInfo.Verb = "runas";

                //cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.EnableRaisingEvents = true;

                foreach (string arg in bcdArgs)
                {
                    cmd.StartInfo.Arguments += arg + " "; // Adds a space after every argument
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

            window.MainFrame.Content = new CleanUpPage(window, raspItem, langjson, true);
        }

        public void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Console.WriteLine(result);
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }

        public void ExecuteCommandAsync(string command)
        {
            try
            {
                //Asynchronously start the Thread to process the Execute command request.
                Thread objThread = new Thread(new ParameterizedThreadStart(ExecuteCommandSync));
                //Make the thread as background thread.
                objThread.IsBackground = true;
                //Set the Priority of the thread.
                objThread.Priority = ThreadPriority.AboveNormal;
                //Start the thread.
                objThread.Start(command);
            }
            catch (ThreadStartException objException)
            {
                // Log the exception
            }
            catch (ThreadAbortException objException)
            {
                // Log the exception
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }
    }
}
