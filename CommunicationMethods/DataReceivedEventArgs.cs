using System;

namespace CommunicationMethods
{
    /// <summary>
    /// Event arguments class passed to handlers of <see cref="ISendReceiveAsync.DataReceived"/>.
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the data received in string format.
        /// </summary>
        /// <remarks>If data cannot be converted to string (ie. non-ASCII byte arrays), <see cref="string.Empty"/> is used.</remarks>
        public string DataString { get; internal set; }

        /// <summary>
        /// Gets the data in byte array format.
        /// </summary>
        public byte[] DataBytes { get; internal set; }
    }
}
