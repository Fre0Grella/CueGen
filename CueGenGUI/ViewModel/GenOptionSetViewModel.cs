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
using Mono.Options;

namespace CueGenGUI.ViewModel
{
    public class GenOptionSetViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<GenOption> OptionSet {  get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public OptionSet options { get; set; }

        public GenOptionSetViewModel()
        {
           
            OptionSet = new ObservableCollection<GenOption>
        {
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption(),
            new GenOption()

            
        };
        }
        

protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}
