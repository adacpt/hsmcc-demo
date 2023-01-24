using System;

namespace Diagnostics.Logging
{
    /// <summary>
    /// Class raises its <see cref="LogEntryCreated"/> event whenever a LogEntry object is created.
    /// </summary>
    /// <remarks>Child classes decide what to do with the event handler from this parent depending
    /// on the business requirements of individual implementations.
    /// For example, one child class may save log entries to a local file.
    /// Others may POST entries to a web server or database. Multiple implementations can exist
    /// within a single application.</remarks>
    public abstract class LogManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogManager"/> class.
        /// </summary>
        public LogManager()
        {
            LogManager.LogEntryCreated += this.NewLogEntryHandler;
        }

        /// <summary>
        /// Event is raised whenever a new LogEntry object is created.
        /// </summary>
        public static event EventHandler<LogEntry> LogEntryCreated;

        /// <summary>
        /// Method raises the <see cref="LogEntryCreated"/> event if there are subscribers. This method is
        /// called as part of the constructor for the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="logEntry">The newly created log entry that will be passed to event subscribers.</param>
        public static void CreateLogEntry(LogEntry logEntry)
        {
            // calls event handler method in new thread
            LogEntryCreated?.Invoke(null, logEntry);
        }

        /// <summary>
        /// Method raises the <see cref="LogEntryCreated"/> event if there are subscribers. This method is
        /// called as part of the constructor for the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="systemName">The system associated with the log entry.</param>
        /// <param name="roomName">The room associated with the log entry.</param>
        /// <param name="deviceName">The device associated with the log entry.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="severityLevel">Severity level of the log entry.</param>
        public static void CreateLogEntry(string systemName, string roomName, string deviceName, string message, SeverityLevel severityLevel)
        {
            CreateLogEntry(new LogEntry(systemName, roomName, deviceName, message, severityLevel));
        }

        /// <summary>
        /// Responds to the <see cref="LogEntryCreated"/> event.
        /// </summary>
        /// <param name="sender">Object that raised the event (always <see cref="LogManager"/>.</param>
        /// <param name="e">The new <see cref="LogEntry"/> that was created.</param>
        protected abstract void NewLogEntryHandler(object sender, LogEntry e);
    }
}
