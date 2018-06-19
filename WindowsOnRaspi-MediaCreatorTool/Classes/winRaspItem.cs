using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsOnRaspi_MediaCreatorTool.Classes
{
    public class WinRaspItem
    {
        // Variables
        public int diskIndex { get; set; }
        public string appFolderPath { get; set; }
        public string winImagePath { get; set; }
        public string raspPiPkgPath { get; set; }
        public string rpiwinStuffPath { get; set; }
        public string rpiDriversPath { get; set; }
        public string sdSize { get; set; }
        public bool isUsingDebugPath { get; set; }
        public bool isUsingUSBBoot { get; set; }
        public string[] tempFolders { get; set; }
    }
}
