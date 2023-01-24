namespace Diagnostics.Logging
{
    /// <summary>
    /// Valid values for a log entry's severity level.
    /// </summary>
    public enum SeverityLevel
    {
        /// <summary>
        /// System is unusable
        /// </summary>
        /// <remarks>A panic condition.</remarks>
        Emergency,

        /// <summary>
        /// Action must be taken immediately
        /// </summary>
        /// <remarks>A condition that should be corrected immediately, such as a corrupted system database.</remarks>
        Alert,

        /// <summary>
        /// Critical conditions
        /// </summary>
        /// <remarks>Hard device errors.</remarks>
        Critical,

        /// <summary>
        /// Error conditions
        /// </summary>
        Error,

        /// <summary>
        /// Warning conditions
        /// </summary>
        Warning,

        /// <summary>
        /// Normal but significant conditions
        /// </summary>
        /// <remarks>Conditions that are not error conditions, but that may require special handling.</remarks>
        Notice,

        /// <summary>
        /// Informational messages
        /// </summary>
        /// <remarks>Confirmation that the program is working as expected.</remarks>
        Informational,

        /// <summary>
        /// Debug-level messages
        /// </summary>
        /// <remarks>Messages that contain information normally of use only when debugging a program.</remarks>
        Debug,
    }
}
