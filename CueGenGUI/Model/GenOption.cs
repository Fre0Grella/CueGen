using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CueGenGUI.Model
{
    public class GenOption
    {
        public bool isExpanded { get; set;}
        public string name { get; set; }
        public string description { get; set; }
        public bool isChecked { get; set; }

        public string args;
        public GenOption(string name, string description, bool active, string options)
        {
            this.name = name;
            this.description = description;
            this.isChecked = active;
            this.args = options;
            this.isExpanded = false;
        }
    }
}
