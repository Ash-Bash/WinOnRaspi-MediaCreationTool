using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for FormatSDCardPage.xaml
    /// </summary>
    public partial class FormatSDCardPage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;
        private dynamic langjson;

        public FormatSDCardPage(MainWindow window, WinRaspItem raspItem, dynamic lang)
        {
            InitializeComponent();

            this.window = window;
            this.raspItem = raspItem;
            this.langjson = lang;

            if (langjson != null)
            {
                titleTextBlock.Text = langjson.pages.formattingSDDrivePage.title;
            }

            this.window.isLockdownMode = true;
            this.window.forceCleanUp = false;

            FormatSDDrive();
        }

        private async void FormatSDDrive() {

            var formatSDTask = Task.Run(() =>
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
                cmd.StandardInput.WriteLine("clean");
                cmd.StandardInput.WriteLine("create partition primary size=400");
                cmd.StandardInput.WriteLine("create partition primary");
                cmd.StandardInput.WriteLine("select partition 1");
                cmd.StandardInput.WriteLine("active");
                cmd.StandardInput.WriteLine("format fs=fat32 quick label=BOOT");
                cmd.StandardInput.WriteLine("assign letter=P");
                cmd.StandardInput.WriteLine("select partition 2");
                cmd.StandardInput.WriteLine("active");
                cmd.StandardInput.WriteLine("format fs=ntfs quick label=Windows");
                cmd.StandardInput.WriteLine("assign letter=I");
                cmd.StandardInput.WriteLine("exit");

                Console.WriteLine("createSCCardPartitions() Process: " + cmd.StandardOutput.ReadToEnd());

                cmd.WaitForExit();
            });

            await formatSDTask;

            window.MainFrame.Content = new HasSDFormatedProperlyPage(window, raspItem, langjson);
           
        }
    }
}
