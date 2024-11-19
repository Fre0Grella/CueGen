using CueGenGUI.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib.Riff;
using CueGenGUI.Model;
using System.Collections.ObjectModel;

namespace CueGenGUI.ViewModel
{
    public class GenOptionSetViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<GenOption> OptionSet {  get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public GenOptionSetViewModel()
        {
            OptionSet = new ObservableCollection<GenOption>
        {
            new GenOption { name = "Phrase", description="make cue in phrase", isChecked = false },
            new GenOption { name = "Attend meeting", description="Lorem ipsum", isChecked = true },
            new GenOption { name = "Exercise", description="zio peppino", isChecked = false }
        };
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}
