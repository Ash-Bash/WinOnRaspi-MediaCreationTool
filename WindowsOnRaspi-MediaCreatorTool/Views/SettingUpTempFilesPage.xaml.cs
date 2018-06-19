using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
    /// Interaction logic for SettingUpTempFilesPage.xaml
    /// </summary>
    public partial class SettingUpTempFilesPage : Page
    {
        // Variables
        private MainWindow window;
        private WinRaspItem raspItem;

        public SettingUpTempFilesPage(MainWindow window, WinRaspItem raspItem)
        {
            InitializeComponent();

            raspItem.appFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "WinOnRaspi-Media-Creator-Tool");
            raspItem.tempFolders = new string[5];

            this.window = window;
            this.raspItem = raspItem;

            SetupTempFiles();
        }

        public async void SetupTempFiles() {

            var setupFolders = Task.Run(() => {
                // Sets up Paths for required folders
                var tempUEFIPath = System.IO.Path.Combine(raspItem.appFolderPath, "temp", "UEFI");
                var tempUUPPath = System.IO.Path.Combine(raspItem.appFolderPath, "temp", "UUP");
                var tempSystem32Path = System.IO.Path.Combine(raspItem.appFolderPath, "temp", "System32");
                var tempExtractedFoldersPath = System.IO.Path.Combine(raspItem.appFolderPath, "temp", "Extracted Folders");
                var tempImagePath = System.IO.Path.Combine(raspItem.appFolderPath, "temp", "Image");

                // Save temp Folders to an array
                raspItem.tempFolders[0] = tempUEFIPath;
                raspItem.tempFolders[1] = tempUUPPath;
                raspItem.tempFolders[2] = tempSystem32Path;
                raspItem.tempFolders[3] = tempExtractedFoldersPath;
                raspItem.tempFolders[4] = tempImagePath;

                // Creates Directories for Required Folders+
                Directory.CreateDirectory(raspItem.tempFolders[0]);
                Directory.CreateDirectory(raspItem.tempFolders[1]);
                Directory.CreateDirectory(raspItem.tempFolders[2]);
                Directory.CreateDirectory(raspItem.tempFolders[3]);
                Directory.CreateDirectory(raspItem.tempFolders[4]);
            });

            await setupFolders;

            var transferZipsTask = Task.Run(() =>
            {
                //Creates First Start Up Reg File
                string firstStartUpString = Properties.Resources.firststartup;
                string instalUEFIString = Properties.Resources.InstallUEFI;
                string signUEFIFilesString = Properties.Resources.SignUEFIFiles;
                string signWindowsString = Properties.Resources.signWindows;
                try
                {
                    File.WriteAllText(System.IO.Path.Combine(raspItem.appFolderPath, "temp") + "/firststartup.reg", firstStartUpString);
                    File.WriteAllText(System.IO.Path.Combine(raspItem.appFolderPath, "temp") + "/InstallUEFI.cmd", instalUEFIString);
                    File.WriteAllText(System.IO.Path.Combine(raspItem.appFolderPath, "temp") + "/SignUEFIFiles.cmd", signUEFIFilesString);

                    File.WriteAllText(raspItem.appFolderPath + "/InstallUEFI.cmd", instalUEFIString);
                    File.WriteAllText(raspItem.appFolderPath + "/SignUEFIFiles.cmd", signUEFIFilesString);
                    File.WriteAllText(raspItem.appFolderPath + "/SignWindows.cmd", signWindowsString);
                }
                catch
                {

                }

                // Extracts all required Zip Files
                try
                {
                    File.Copy(raspItem.winImagePath, System.IO.Path.Combine(raspItem.tempFolders[1], "Windows10-Arm64.iso"));
                }
                catch {

                }

                try
                {
                    ZipFile.ExtractToDirectory(raspItem.raspPiPkgPath, raspItem.tempFolders[3]);
                }
                catch
                {

                }

                try
                {
                    ZipFile.ExtractToDirectory(raspItem.rpiwinStuffPath, raspItem.tempFolders[3]);
                }
                catch
                {

                }

                // Rename Folders in the Extracted Folders
                var extrFolders = Directory.GetDirectories(raspItem.tempFolders[3]);
                try
                {
                    Directory.Move(extrFolders[0], System.IO.Path.Combine(raspItem.appFolderPath, "temp", "Extracted Folders", "RaspberryPiPkg"));
                } catch
                {

                }

                try
                {
                    Directory.Move(extrFolders[1], System.IO.Path.Combine(raspItem.appFolderPath, "temp", "Extracted Folders", "winOnRaspi"));
                } catch
                {

                }

                return true;
            });

            await transferZipsTask;

            DirectoryInfo extFolders = new DirectoryInfo(raspItem.tempFolders[1]);
            FileInfo[] fileInfo = extFolders.GetFiles();

            Debug.WriteLine(System.IO.Path.Combine(raspItem.tempFolders[1], fileInfo[0].ToString()));
            var isoPath = System.IO.Path.Combine(raspItem.tempFolders[1], fileInfo[0].ToString());

            string driveLetter = null;

            var isoMountTask = Task.Run(() => {
                using (var ps = PowerShell.Create())
                {
                    var command = ps.AddCommand("Mount-DiskImage");
                    command.AddParameter("ImagePath", isoPath);
                    command.Invoke();
                    ps.Commands.Clear();

                    //Get Drive Letter ISO Image Was Mounted To
                    var runSpace = ps.Runspace;
                    var pipeLine = runSpace.CreatePipeline();
                    var getImageCommand = new Command("Get-DiskImage");
                    getImageCommand.Parameters.Add("ImagePath", isoPath);
                    pipeLine.Commands.Add(getImageCommand);
                    pipeLine.Commands.Add("Get-Volume");

                    foreach (PSObject psObject in pipeLine.Invoke())
                    {
                        if (psObject != null)
                        {
                            driveLetter = psObject.Members["DriveLetter"].Value.ToString();
                            Console.WriteLine("Mounted On Drive: " + driveLetter);
                        }
                    }
                }
            });

            await isoMountTask;

            var copyInstallWinTask = Task.Run(() => {
                if (File.Exists(driveLetter + @":\sources\install.wim"))
                {
                    Debug.WriteLine("File Exists: " + true);
                    Debug.WriteLine("File Path: " + driveLetter + @":\sources\install.wim");

                    string[] dismArgs = new string[3];
                    dismArgs[0] = "/Export-Image /SourceImageFile:" + driveLetter + @":\sources\install.wim /SourceIndex:1 /DestinationImageFile:" + System.IO.Path.Combine(raspItem.appFolderPath, "temp", "install.wim");

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

                }
                else
                {
                    Debug.WriteLine("File Exists: " + false);
                    window.MainFrame.Content = new CleanUpPage(window, raspItem, false);
                }
            });

            await copyInstallWinTask;

            var packagesTask = Task.Run(() =>
            {
                var driverPath = System.IO.Path.Combine(raspItem.tempFolders[3], "winOnRaspi", "driver_prebuilts");
                CopyFilesRecursively(new DirectoryInfo(driverPath), new DirectoryInfo(raspItem.tempFolders[2]));

                var driver2Path = System.IO.Path.Combine(raspItem.tempFolders[3], "winOnRaspi", "winpe_stuff");
                CopyFilesRecursively(new DirectoryInfo(driver2Path), new DirectoryInfo(raspItem.tempFolders[2]));
            });

            await packagesTask;

            var isoUnmountTask = Task.Run(() => {
                using (var ps = PowerShell.Create())
                {
                    //Unmount Via Image File Path
                    var command = ps.AddCommand("Dismount-DiskImage");
                    command.AddParameter("ImagePath", isoPath);
                    ps.Invoke();
                    ps.Commands.Clear();
                }
            });

            await isoUnmountTask;

            window.MainFrame.Content = new SelectedRecommendedDriversPage(window, raspItem);

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
