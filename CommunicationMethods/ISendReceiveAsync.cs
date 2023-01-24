using System;
using Diagnostics.Debugging;

namespace CommunicationMethods
{
    /// <summary>
    /// Defines the behavior for asynchronously sending/receiving data. Implements <see cref="ISendReceiveDebuggable"/>.
    /// </summary>
    public interface ISendReceiveAsync : ISendReceiveDebuggable
    {
        /// <summary>
        /// Raised when the object asynchronously receives data.
        /// </summary>
        event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Gets or sets a value indicating whether data communication is enabled.
        /// </summary>
        bool Enable { get; set; }

        /// <summary>
        /// Sends data in string format.
        /// </summary>
        /// <param name="data">Data to send.</param>
        void SendString(string data);

        /// <summary>
        /// Sends data in byte-array format.
        /// </summary>
        /// <param name="data">Data to send.</param>
        void SendBytes(byte[] data);
    }
}
