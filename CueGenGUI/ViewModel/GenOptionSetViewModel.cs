using CueGenGUI.Model;
using CueGen.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib.Riff;
using System.Collections.ObjectModel;
using Mono.Options;
using System.Windows.Input;

namespace CueGenGUI.ViewModel
{
    public class GenOptionSetViewModel : INotifyPropertyChanged
    {
        
        public ObservableCollection<GenOption> OptionSet {  get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public OptionSet options { get; set; }
        public ICommand ExecuteCommandsCommand { get; }

        public GenOptionSetViewModel()
        {
           
            OptionSet = new ObservableCollection<GenOption>
        {
            new GenOption("Test run","Do not alter Rekordbox database, only perform a test run",false,"dryrun"),
            new GenOption("hotcues","Create hot cue points instead of memory cue points",false,"hc|hotcues"),
            new GenOption("merge","Merge with existing cue points (default is enabled)",true,"m|merge"),
            new GenOption("distance","Minimum distance in bars to existing cue points (no tick have 4 has default)",false,"d|distance="),
            new GenOption("colors","Comma separated list of hot cue colors, same order as in Rekordbox, top left is 1 (no tick have 1, 4, 6, 9, 12, 13, 14, 15 as default)",false,"colors="),
            new GenOption("max cue number","Maximum number of cue points to create (no tick have 8 as default)",false,"x|max="),
            new GenOption("comment","Comment template, # will be replaced by energy level (no tick have \"Energy #\" as default)",false,"comment="),
            new GenOption("glob fiel check","File glob of track file paths to include, e.g. C:\\Music\\*.mp3 (no tick have all in Rekordbox database) as default",false,"g|glob="),
            new GenOption("remove","Remove all cue points created through this program",false,"r|remove"),
            new GenOption("backup","Create database backup before creating cue points (default is enabled)",true,"b|backup"),
            new GenOption("snap to grid","Snap cue points to nearest bar (default is enabled)",true,"s|snap"),
            new GenOption("phrase","Create cue points from phrases (default is disabled)",false,"p|phrase"),
            new GenOption("my tag","Create MyTag with energy level (default is disabled)",false,"my|mytag"),
            new GenOption("energy track","Set track color according to energy level (default is disabled)",false,"e|energy"),
            new GenOption("energy color","Set cue point color according to cue point's energy level (default is enabled)",true,"energycolor"),
            new GenOption("phrase color","Set cue point color according to cue point's phrase (default is enabled)",true,"phrasecolor"),
            new GenOption("progress","Report progress (default is enabled)",true,"progress"),
            new GenOption("define database","File path to Rekordbox database (no tick have autodetect as default)",false,"db|database="),
            new GenOption("phrase name","Phrase names, comma separated (default are Intro, Verse, Bridge, Chorus, Outro, Up, Down)",false,"n|names="),
            new GenOption("change grouping order","Phrase group order, comma separated groups of slash separated phrase names (no tick have Intro, Outro, Verse, Chorus, Bridge, Up/Down) as default",false,"o|order="),
            new GenOption("minimum phrase length", "Minimum length of phrase group in bars (no tick have 4 as default)",false,"phraselength="),
            new GenOption("active intro loop","Length in beats of active loop intro (default is disabled)",false,"li|loopintro="),
            new GenOption("active outro loop","Length in beats of active loop outro (default is disabled)",false,"lo|loopoutro="),
            new GenOption("offset by cue","Number of beats to offset cue points by, may be negative (no tick have 0 as default)",false,"of|offset="),     
        };
        }

        private void OnExecuteClicked(object sender, EventArgs e)
        {
            // Filtra i comandi selezionati
            var selectedCommands = OptionSet.Where(c => c.isChecked).ToList();

            /*if (!selectedCommands.Any())
            {
                ResultLabel.Text = "Nessun comando selezionato!";
                ResultLabel.TextColor = Colors.Red;
                return;
            }*/

            // Esegui la logica per i comandi selezionati
            var logic = new Program();
            var args = selectedCommands.Select(cmd => cmd.args);
            logic.Generate(args.ToList());
            // Mostra i risultati
            //var progressBar = new ProgressBar();

            //ResultLabel.Text = string.Join("\n", results);
            //ResultLabel.TextColor = Colors.Green;
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}
