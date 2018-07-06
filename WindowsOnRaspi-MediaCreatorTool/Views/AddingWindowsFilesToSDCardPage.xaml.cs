using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
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
    /// Interaction logic for AddingWindowsFilesToSDCardPage.xaml
    /// </summary>
    public partial class AddingWindowsFilesToSDCardPage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;
        private dynamic langjson;

        public AddingWindowsFilesToSDCardPage(MainWindow window, WinRaspItem raspItem, dynamic lang)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;
            this.langjson = lang;

            if (langjson != null && langjson.pages.addingWindowsFilesToSDCardPage != null)
            {
                titleTextBlock.Text = langjson.pages.addingWindowsFilesToSDCardPage.title;
                subtitleTextBlock.Text = langjson.pages.addingWindowsFilesToSDCardPage.subtitle;
            }

            this.window.isLockdownMode = true;
            this.window.forceCleanUp = true;
            this.window.raspItem = raspItem;

            AddingWindowsFiles();    
        }

        private async void AddingWindowsFiles() {

            var cdPath = System.IO.Path.Combine(raspItem.appFolderPath, "temp");
            var wimpath = System.IO.Path.Combine(cdPath, "install.wim");

            if (File.Exists(wimpath))
            {
                Debug.WriteLine("install.wim Path Exists: " + true);

                var addingWindowsFilesTask = Task.Run(() =>
                {
                    string[] dismArgs = new string[3];
                    dismArgs[0] = "/apply-image /imagefile:install.wim /index:1 /applydir:" + @"I:";
                    //dismArgs[0] = "wimlib-imagex apply install.wim 1 i:";
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

                    Console.WriteLine(cmd.StandardOutput.ReadToEnd());

                    cmd.WaitForExit();

                    CopyFilesRecursively(new DirectoryInfo(raspItem.tempFolders[0]), new DirectoryInfo("P:/"));
                });

                await addingWindowsFilesTask;

                File.Copy(System.IO.Path.Combine(cdPath, "firststartup.reg"), "I:/firststartup.reg");

                //window.MainFrame.Content = new SigningWindowsFilesPage(window, raspItem);
                window.MainFrame.Content = new CleanUpPage(window, raspItem, langjson, true);

            } else
            {
                window.MainFrame.Content = new CleanUpPage(window, raspItem, langjson, false);
            }

        }

        // Allows Copying files and Folders from one Dir to Another
        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                foreach (DirectoryInfo dir in source.GetDirectories())
                    CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
                foreach (FileInfo file in source.GetFiles())
                    file.CopyTo(System.IO.Path.Combine(target.FullName, file.Name));
            }
            catch (Exception e)
            {
                Debug.WriteLine("Somewthing Went Wrong!! " + e);
            }
        }
    }
}
