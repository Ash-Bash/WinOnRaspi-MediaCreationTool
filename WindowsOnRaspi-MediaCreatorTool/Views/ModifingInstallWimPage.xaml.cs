using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
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
    /// Interaction logic for ModifingInstallWimPage.xaml
    /// </summary>
    public partial class ModifingInstallWimPage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;
        private dynamic langjson;

        public ModifingInstallWimPage(MainWindow window, WinRaspItem raspItem, dynamic lang)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;
            this.langjson = lang;

            if (langjson != null && langjson.pages.modifingInstallWimPage != null)
            {
                titleTextBlock.Text = langjson.pages.modifingInstallWimPage.title;
                subtitleTextBlock.Text = langjson.pages.modifingInstallWimPage.subtitle;
            }

            this.window.isLockdownMode = true;
            this.window.forceCleanUp = true;
            this.window.raspItem = raspItem;

            ModifingInstallWim();
        }

        private async void ModifingInstallWim() {

            var cdPath = System.IO.Path.Combine(raspItem.appFolderPath, "temp");
            var wimpath = System.IO.Path.Combine(cdPath, "install.wim");

            if (File.Exists(wimpath))
            {

                Debug.WriteLine("install.wim Path Exists: " + true);

                var mountInstallWimTask = Task.Run(() =>
                {
                    string[] dismArgs = new string[3];
                    dismArgs[0] = "/mount-image /imagefile:" + wimpath + " /Index:1 /MountDir:" + System.IO.Path.Combine(cdPath, "Image");

                    Process cmd = new Process();
                
                    if (Environment.Is64BitOperatingSystem)
                    {
                        cmd.StartInfo.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SysWOW64", "dism.exe");
                    }
                    else
                    {
                        cmd.StartInfo.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "dism.exe");
                    }

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

                    Console.WriteLine("mountInstallWimFile() Process: " + cmd.StandardOutput.ReadToEnd());

                    cmd.WaitForExit();

                });

                await mountInstallWimTask;

                var addDriversToInstallWimTask = Task.Run(() =>
                {
                    string[] dismArgs = new string[3];
                    dismArgs[0] = "/image:" + System.IO.Path.Combine(cdPath, "Image") + " /add-driver /driver:" + System.IO.Path.Combine(cdPath, "system32") + " /recurse /forceunsigned";

                    Process cmd = new Process();
                    if (Environment.Is64BitOperatingSystem)
                    {
                        cmd.StartInfo.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SysWOW64", "dism.exe");
                    }
                    else
                    {
                        cmd.StartInfo.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "dism.exe");
                    }
                    cmd.StartInfo.WorkingDirectory = cdPath;
                    cmd.StartInfo.Verb = "runas";

                    foreach (string arg in dismArgs)
                    {
                        cmd.StartInfo.Arguments += arg;
                    }

                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.EnableRaisingEvents = true;


                    cmd.Start();

                    Console.WriteLine("addDriversInstallWimFile() Process: " + cmd.StandardOutput.ReadToEnd());

                    cmd.WaitForExit();

                });

                await addDriversToInstallWimTask;

                var unmountInstallWimTask = Task.Run(() =>
                {
                    string[] dismArgs = new string[3];
                    dismArgs[0] = "/unmount-wim /mountdir:" + System.IO.Path.Combine(cdPath, "Image") + " /commit";

                    Process cmd = new Process();
                    if (Environment.Is64BitOperatingSystem)
                    {
                        cmd.StartInfo.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SysWOW64", "dism.exe");
                    }
                    else
                    {
                        cmd.StartInfo.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "dism.exe");
                    }
                    cmd.StartInfo.WorkingDirectory = cdPath;
                    cmd.StartInfo.Verb = "runas";

                    foreach (string arg in dismArgs)
                    {
                        cmd.StartInfo.Arguments += arg;
                    }

                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.EnableRaisingEvents = true;

                    cmd.Start();

                    Console.WriteLine("unmountInstallWimFile() Process: " + cmd.StandardOutput.ReadToEnd());

                    cmd.WaitForExit();

                });

                await unmountInstallWimTask;

                window.MainFrame.Content = new AddingWindowsFilesToSDCardPage(window, raspItem, langjson);

            } else
            {
                window.MainFrame.Content = new CleanUpPage(window, raspItem, langjson, false);
            }

        }
    }
}
