using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Crestron.SimplSharp;

namespace Diagnostics.Logging
{
    /// <summary>
    /// Class logs CSV files to 'user' folder (by default) on Crestron 4-series processors.
    /// Default file name is app0XMaveLog.csv.
    /// Creates custom console command 'LogSeverityThreshold' for changing the threshold for logging entries.
    /// </summary>
    public class Crestron4SeriesCsvLog : LogToCsvFile
    {
        // command name for Crestron console command
        private string logSeverityConsoleCommand = GetAppNumber() + "LogThreshold";

        // help string for Crestron console command
        private string logSeverityHelp = "Set the severity level for events to be logged.";

        /// <summary>
        /// Initializes a new instance of the <see cref="Crestron4SeriesCsvLog"/> class.
        /// </summary>
        public Crestron4SeriesCsvLog()
        {
            ErrorLog.Notice("Attempting to create custom console command.");
            try
            {
                // creates a new console command in Crestron to set the severity level at which new LogEntry objects
                // will be added to the log file
                CrestronConsole.AddNewConsoleCommand(
                    this.SetSeverityThresholdCallback,
                    this.logSeverityConsoleCommand,
                    this.logSeverityHelp,
                    ConsoleAccessLevelEnum.AccessOperator);
            }
            catch (Exception e)
            {
                ErrorLog.Notice($"Error creating log severity console command: {e.Message}");
            }
        }

        /// <inheritdoc/>
        public override string FileName { get; set; } = GetFileName();

        /// <inheritdoc/>
        // sets default file path to the 'user' folder on the 4-series processor
        public override string FilePath { get; set; } = "\\user\\";

        private static string ListOfSeverityLevels()
        {
            // gets string list of all potential values in SeverityLevel enum
            var levels = Enum.GetValues(typeof(SeverityLevel))
                .Cast<SeverityLevel>()
                .Select(v => v.ToString())
                .ToList();

            StringBuilder sb = new StringBuilder();

            // append all values to a string and return
            for (int i = 0; i < levels.Count; i++)
            {
                sb.Append(levels[i]);

                if (i < levels.Count - 1)
                    sb.Append("|");
            }

            return sb.ToString();
        }

        private static string GetFileName()
        {
            return GetAppNumber() + "MaveLog.csv";
        }

        private static string GetAppNumber()
        {
            try
            {
                // executable file path is: /simpl/app02/SimplSharpProProgramName.dll
                var execPath = Assembly.GetExecutingAssembly().Location;
                var directories = execPath.Split('/');
                string appNumber = directories[2];
                return appNumber;
            }
            catch (Exception e)
            {
                ErrorLog.Notice($"Error changing default error log file name: {e.Message}");
            }

            return string.Empty;
        }

        private void SetSeverityThresholdCallback(string parameters)
        {
            // if parameters are entered then try to set a new severity threshold
            if (parameters.Length > 1)
            {
                // try to parse parameters to SeverityLevel enum
                if (Enum.TryParse(parameters, out SeverityLevel newLevel))
                {
                    this.SeverityThreshold = newLevel;
                    CrestronConsole.ConsoleCommandResponse($"Log severity threshold set to {this.SeverityThreshold}");
                }
                else
                {
                    CrestronConsole.ConsoleCommandResponse($"Invalid entry for new log severity threshold: {parameters}\n" +
                        $"Valid choices are: {ListOfSeverityLevels()}");
                }
            }

            // otherwise show the user the current threshold
            else
            {
                CrestronConsole.ConsoleCommandResponse($"Current severity threshold: {this.SeverityThreshold}");
            }
        }
    }
}