using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Diagnostics.Logging
{
    /// <summary>
    /// Saves log entries to a CSV file.
    /// </summary>
    public class LogToCsvFile : LogManager
    {
        /// <summary>
        /// Gets or sets the severity level at which log entries will be saved to the file.
        /// Default value is <see cref="SeverityLevel.Error"/>.
        /// </summary>
        public SeverityLevel SeverityThreshold { get; set; } = SeverityLevel.Error;

        /// <summary>
        /// Gets or sets the path to folder where log file will be saved.
        /// Default is location of program executable.
        /// </summary>
        public virtual string FilePath { get; set; } = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString() + "\\";

        /// <summary>
        /// Gets or sets the file name for the log file.
        /// </summary>
        public virtual string FileName { get; set; } = "MaveLog.csv";

        /// <inheritdoc/>
        protected override void NewLogEntryHandler(object sender, LogEntry e)
        {
            if (e.SeverityLevel <= this.SeverityThreshold)
            {
                // check if log file exists
                if (File.Exists(this.FilePath + this.FileName))
                    this.AppendLogEntryToFile(e);
                else this.CreateNewLogFile(e);
            }
        }

        // appends a CSV-formatted string to the end of the file
        private void AppendLogEntryToFile(LogEntry logEntry)
        {
            File.AppendAllText(this.FilePath + this.FileName, this.CsvLogEntryLine(logEntry));
        }

        // starts a new CSV file with header row and first log entry
        private void CreateNewLogFile(LogEntry logEntry)
        {
            File.WriteAllText(this.FilePath + this.FileName, this.CsvHeaderLine(logEntry) + this.CsvLogEntryLine(logEntry));
        }

        // generates the CSV-formatted string with all log entry property values
        private string CsvLogEntryLine(LogEntry logEntry)
        {
            // get all properties in LogEntry type
            var props = logEntry.GetType().GetProperties();

            StringBuilder sb = new StringBuilder();

            // iterate through each property, appending its value to the CSV line
            for (int i = 0; i < props.Length; i++)
            {
                // enclose values in double quotes to account for any commas in data fields
                sb.Append($"\"{props[i].GetValue(logEntry)}\"");

                // if we are not on the last iteration, append a comma
                if (i < props.Length - 1)
                    sb.Append(",");

                // otherwise append a new line
                else sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        // generates the CSV-formatted string will all log entry property names
        private string CsvHeaderLine(LogEntry logEntry)
        {
            var props = logEntry.GetType().GetProperties();
            var sb = new StringBuilder();

            // iterate through properties, appending its name to the CSV line
            for (int i = 0; i < props.Length; i++)
            {
                sb.Append(props[i].Name);
                if (i < props.Length - 1)
                    sb.Append(",");
                else sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
