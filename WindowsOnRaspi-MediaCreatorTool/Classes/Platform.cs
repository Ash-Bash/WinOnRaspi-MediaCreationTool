using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsOnRaspi_MediaCreatorTool.Classes
{
    public class Platform
    {

        public static bool IsWinNT() {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                return true;
            } else
            {
                return false;
            }
        }
        public static bool IsCreatorOrBetter() {
            Debug.WriteLine("Windows OS Version: {0}", Environment.OSVersion.ToString());
            if (Environment.OSVersion.Version.Major >= 10)
            {
                if (Environment.OSVersion.Version.Build >= 15063)
                {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        }

        public static bool IsWindows10()
        {
            if (Environment.OSVersion.Version.Major >= 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
