using System;
using System.Diagnostics;
using System.Text;

namespace Diagnostics.Logging
{
    /// <summary>
    /// Contains all required data for a syslog entry. Instances can only be created using the
    /// static <see cref="CreateLogEntry(string, string, string, string, SeverityLevel)"/> method.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="systemName">The system associated with the log entry.</param>
        /// <param name="roomName">The room associated with the log entry.</param>
        /// <param name="deviceName">The device associated with the log entry.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="severityLevel">Severity level of the log entry.</param>
        public LogEntry(string systemName, string roomName, string deviceName, string message, SeverityLevel severityLevel)
        {
            this.SystemName = systemName;
            this.RoomName = roomName;
            this.DeviceName = deviceName;
            this.Message = message;
            this.SeverityLevel = severityLevel;

            // offset of 2 ignores this constructor and WhoCreatedMe method
            this.Origin = this.WhoCreatedMe(2);
        }

        /// <summary>
        /// Gets the unique identifier of the log entry.
        /// </summary>
        public Guid Guid { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the date/time the log entry was created.
        /// </summary>
        public DateTime CreationTime { get; } = DateTime.Now;

        /// <summary>
        /// Gets the severity level of the log entry.
        /// </summary>
        public SeverityLevel SeverityLevel { get; }

        /// <summary>
        /// Gets the system ID associated with the log entry.
        /// </summary>
        public string SystemName { get; }

        /// <summary>
        /// Gets the room ID associated with the log entry.
        /// </summary>
        public string RoomName { get; }

        /// <summary>
        /// Gets the room ID associated with the log entry.
        /// </summary>
        public string DeviceName { get; }

        /// <summary>
        /// Gets the log entry message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the origin (stack trace) for the creation of the log entry.
        /// </summary>
        public string Origin { get; }

        private string WhoCreatedMe(uint offset)
        {
            // create stack strack object
            StackTrace stackTrace = new StackTrace();

            // get the stack frames for this method
            var frames = stackTrace.GetFrames();

            // create string builder to generate the loggable origin string
            StringBuilder sb = new StringBuilder();

            // iterate through stack frames, starting with this top-level and working downward
            for (int i = frames.Length - 1; i > offset - 1; i--)
            {
                var method = frames[i].GetMethod();
                var parameters = method.GetParameters();
                var className = method.DeclaringType.Name;

                // create a new SB to build the formatted list of method parameters
                StringBuilder paramString = new StringBuilder();

                // iterate through each parameter and the Type and Name to the string
                for (int j = 0; j < parameters.Length; j++)
                {
                    paramString.Append($"{parameters[j].ParameterType.Name} {parameters[j].Name}");

                    // add a comma if not the last parameter
                    if (j < parameters.Length - 1)
                        paramString.Append(",");
                }

                // append the frame elements to the origin trace
                sb.Append($"{className}.{method.Name}({paramString.ToString()})");

                // add a -> if not the last one
                if (i > offset)
                    sb.Append(" -> ");
            }

            return sb.ToString();
        }
    }
}
