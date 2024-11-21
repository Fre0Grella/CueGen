using CueGen.Analysis;
using Mono.Options;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

namespace CueGen.Core
{
    public class Program
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        readonly Config Config = new();
        bool Error = false;
        bool Backup = true;
        bool ReportProgress = true;

        public Program()
        {
        }

        public int Generate(List<string> args)
        {
            try
            {
               
                var colors = "";
                var logPath = "";
                var logLevel = LogLevel.Warn;
                var phraseNames = "";
                var phraseOrder = "";

                try
                {
                    var options = new OptionSet
                    {
                        { "dryrun", "Do not alter Rekordbox database, only perform a test run", v => this.Config.DryRun = v != null },
                        { "hc|hotcues", "Create hot cue points instead of memory cue points", v => this.Config.HotCues = v != null },
                        { "m|merge", "Merge with existing cue points (default is enabled)", v => this.Config.Merge = v != null },
                        { "d|distance=", "Minimum distance in bars to existing cue points (default is 4)", (int v) => this.Config.MinDistanceBars = v },
                        { "colors=", "Comma separated list of hot cue colors, same order as in Rekordbox, top left is 1 (default is 1, 4, 6, 9, 12, 13, 14, 15)", v => colors = v },
                        { "x|max=", "Maximum number of cue points to create (default is 8)", (int v) => this.Config.MaxCues = v },
                        { "comment=", "Comment template, # will be replaced by energy level (default is \"Energy #\")", v => this.Config.Comment = v },
                        { "g|glob=", "File glob of track file paths to include, e.g. C:\\Music\\*.mp3 (default is all in Rekordbox database)", v => this.Config.FileGlob = v },
                        { "r|remove", "Remove all cue points created through this program", v => this.Config.RemoveOnly = v != null },
                        { "b|backup", "Create database backup before creating cue points (default is enabled)", v => this.Backup = v != null },
                        { "s|snap", "Snap cue points to nearest bar (default is enabled)", v => this.Config.SnapToBar = v != null },
                        { "p|phrase", "Create cue points from phrases (default is disabled)", v => this.Config.PhraseCues = v != null },
                        { "my|mytag", "Create MyTag with energy level (default is disabled)", v => this.Config.MyTagEnergy = v != null },
                        { "e|energy", "Set track color according to energy level (default is disabled)", v => this.Config.ColorEnergy = v != null },
                        { "energycolor", "Set cue point color according to cue point's energy level (default is enabled)", v => this.Config.CueColorEnergy = v != null },
                        { "phrasecolor", "Set cue point color according to cue point's phrase (default is enabled)", v => this.Config.CueColorPhrase = v != null },
                        { "progress", "Report progress (default is enabled)", v => this.ReportProgress = v != null },
                        { "db|database=", "File path to Rekordbox database (default is autodetect)", v => this.Config.DatabasePath = v },
                        { "v|verbosity=", "Verbosity level (default is warn, possible values are off, fatal, error, warn, info, debug, trace)", v => logLevel = LogLevel.FromString(v) },
                        { "n|names=", "Phrase names, comma separated (default are Intro,Verse,Bridge,Chorus,Outro,Up,Down)", v => phraseNames = v },
                        { "o|order=", "Phrase group order, comma separated groups of slash separated phrase names (default is Intro,Outro,Verse,Chorus,Bridge,Up/Down)", v => phraseOrder = v },
                        { "phraselength=", "Minimum length of phrase group in bars (default is 4)", (int v) => this.Config.MinPhraseLength = v },
                        { "mindate=", "Minimum creation date of tracks (default is any, format is 2021-12-32T23:31:00, time is optional)", v => this.Config.MinCreatedDate =
                            DateTime.Parse(v, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal) },
                        { "li|loopintro=", "Length in beats of active loop intro (default is disabled)", (int v) => this.Config.LoopIntroLength = v },
                        { "lo|loopoutro=", "Length in beats of active loop outro (default is disabled)", (int v) => this.Config.LoopOutroLength = v },
                        { "of|offset=", "Number of beats to offset cue points by, may be negative (default is 0)", (int v) => this.Config.CueOffset = v },
                    };

                    options.Parse(args);



                    if (!string.IsNullOrEmpty(logPath))
                    {
                        var logFile = new FileTarget("file")
                        {
                            Encoding = Encoding.UTF8,
                            FileName = logPath,
                            Layout = "${longdate}|${level:uppercase=true}|${message} ${exception:format=toString,Data}",
                            DeleteOldFileOnStartup = true
                        };
                    }


                    if (!string.IsNullOrEmpty(colors))
                        this.Config.Colors = colors.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(c => int.Parse(c.Trim()) - 1).Where(i => i >= 0 && i < 16).ToList();

                    if (!string.IsNullOrEmpty(phraseNames))
                        this.Config.PhraseNames = phraseNames.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Take(Enum.GetValues<PhraseGroup>().Length)
                            .Select((n, i) => (Index: i, Name: n))
                            .ToDictionary(n => (PhraseGroup)(n.Index + 1), n => n.Name);

                    if (!string.IsNullOrEmpty(phraseOrder))
                    {
                        var phraseGroups = Enum.GetNames<PhraseGroup>().ToDictionary(n => n, n => Enum.Parse<PhraseGroup>(n));

                        this.Config.PhraseOrder = phraseOrder.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select((n, i) => (Index: i, Names: n.Split('/', StringSplitOptions.RemoveEmptyEntries)))
                            .SelectMany(e => e.Names.Select(n => (e.Index, Name: n)))
                            .ToDictionary(n => phraseGroups.Single(g => g.Key.StartsWith(n.Name, StringComparison.OrdinalIgnoreCase)).Value, n => n.Index);
                    }

                    static void CheckLoopLength(int l)
                    {
                        if ((l & (l - 1)) != 0 || l < 0 || l > 512)
                            throw new ArgumentException("Loop length must be a power of 2 and less than or equal to 512");
                    }

                    CheckLoopLength(this.Config.LoopIntroLength);
                    CheckLoopLength(this.Config.LoopOutroLength);

                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Error parsing command line arguments");
                    return 1;
                }

                this.Generate();

                return this.Error ? 1 : 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An error has occurred");
                return 2;
            }
        }

      

        static string Version => typeof(Generator).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

        void Generate()
        {
            Log.Info("Starting CueGen version {version}", Version);

            if (string.IsNullOrEmpty(Config.DatabasePath))
            {
                var db = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                    Environment.ExpandEnvironmentVariables(@"%AppData%\Pioneer\rekordbox\master.db") :
                    Environment.ExpandEnvironmentVariables("%HOME%/Library/Pioneer/rekordbox/master.db");
                if (!File.Exists(db))
                {
                    Log.Error("Rekordbox database not found at {database}", db);
                    throw new FileNotFoundException("Rekordbox database not found", db);
                }
                else
                    Log.Info("Rekordbox database is at {database}", db);

                Config.DatabasePath = db;
            }

            if (Backup)
            {
                var dir = Path.GetDirectoryName(Config.DatabasePath);
                var fn = Path.GetFileNameWithoutExtension(Config.DatabasePath);
                var backupPath = Path.Combine(dir, $"{fn}.backup.{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.db");
                Log.Info("Creating database backup at {path}", backupPath);
                File.Copy(Config.DatabasePath, backupPath);
            }

            var generator = new Generator(Config);

            if (!generator.Generate())
                Error = true;
        }
    }
}

