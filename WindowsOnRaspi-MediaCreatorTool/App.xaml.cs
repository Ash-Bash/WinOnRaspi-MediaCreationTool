using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WindowsOnRaspi_MediaCreatorTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() {

            this.InitializeComponent();

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "WinOnRaspi-Media-Creator-Tool", "temp");

            if (Directory.Exists(path))
            {
                //Exists
            }
            else
            {
                //Needs to be created
                Directory.CreateDirectory(path);
            }
        }
    }

}
