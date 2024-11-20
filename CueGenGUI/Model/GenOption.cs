using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CueGenGUI.Model
{
    public class GenOption
    {

        public string name { get; set; }
        public string description { get; set; }
        public bool isChecked { get; set; }

        List<string>? options;
        public GenOption(string name, string description, bool active, List<string>? options)
        {
            this.name = name;
            this.description = description;
            this.isChecked = active;
        }
    }
}
