using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
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
using WindowsOnRaspi_MediaCreatorTool.Views;

namespace WindowsOnRaspi_MediaCreatorTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Variables
        public Page mainPage;
        public bool isLockdownMode = false;
        public bool forceCleanUp = false;
        public WinRaspItem raspItem;

        private Page cleanuppage;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            MainFrame.Content = new StartUpPage(this);

            // Automaticly Enables Start as Admin (Comment it if you want to debug application aka read the output)
            AdminRelauncher();

        }

        private void AdminRelauncher()
        {
            if (!IsRunAsAdmin())
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Assembly.GetEntryAssembly().CodeBase;

                proc.Verb = "runas";

                try
                {
                    Process.Start(proc);
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("This program must be run as an administrator! \n\n" + ex.ToString());
                }
            }
        }

        private bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (isLockdownMode)
            {
                e.Cancel = true;
                MessageBox.Show("You Can't Close This Application at this time because the application is running an important task that may corrupt your system.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            } else
            {
                if (forceCleanUp)
                {
                    e.Cancel = true;

                    if (MessageBox.Show("If You Press Yes It will Clean Up and Close Application, Do you want to Continue?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        // user clicked yes
                        MainFrame.Content = new CleanUpPage(this, raspItem, false, true);
                    }
                    else
                    {
                        // user clicked no
                    }

                }
                else
                {
                    if (MessageBox.Show("This Action will stop the installation process, Do you want to Continue?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        // user clicked yes
                        e.Cancel = false;
                    }
                    else
                    {
                        // user clicked no
                        e.Cancel = true;
                    }
                }
            }
        }

        public void SetPage(Page page) {
            cleanuppage = page;
        }

        private Page GetPage() {
            return cleanuppage;
        }
    }
}
