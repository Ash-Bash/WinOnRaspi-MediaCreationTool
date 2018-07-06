using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsOnRaspi_MediaCreatorTool.Classes
{
    public class LangListItem
    {
        // Variables
        public string title { get; set; }
        public string lang { get; set; }
        public string link { get; set; }

        public LangListItem() {

        }

        public LangListItem(string title, string lang, string link) {
            this.title = title;
            this.lang = lang;
            this.link = link;
        }
    }
}
