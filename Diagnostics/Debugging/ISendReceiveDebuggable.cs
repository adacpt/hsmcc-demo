using System;

namespace Diagnostics.Debugging
{
    public interface ISendReceiveDebuggable
    {
        /// <summary>
        /// Event is raised when the implementation class sends data.
        /// String representation of the data is sent to the event handler.
        /// </summary>
        event EventHandler<string> DebugDataSent;

        /// <summary>
        /// Event is raised when the implementation class receives data.
        /// String representation of the data is sent to the event handler.
        /// </summary>
        event EventHandler<string> DebugDataReceived;

        /// <summary>
        /// Gets or sets the name for the object that will be referenced in the debugging environment.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether debugging is enabled for this object.
        /// </summary>
        bool EnableDebug { get; set; }

        /// <summary>
        /// Raises the <see cref="DebugDataSent"/> event.
        /// </summary>
        /// <param name="data">String representation of the data sent.</param>
        void OnDebugDataSent(string data);

        /// <summary>
        /// Raises the <see cref="DebugDataReceived"/> event.
        /// </summary>
        /// <param name="data">String representation of the data received.</param>
        void OnDebugDataReceived(string data);

        /// <summary>
        /// Allows the debugging environment to send a string of data.
        /// </summary>
        /// <param name="data">String to send.</param>
        void SendDataFromDebug(string data);

        /// <summary>
        /// Allows the debugging environment to send a byte array of data.
        /// </summary>
        /// <param name="data">Bytes to send.</param>
        void SendDataFromDebug(byte[] data);

        /// <summary>
        /// Adds the implementation object to <see cref="DebugEnvironment.DebuggableObjects"/>.
        /// Should be called when the implementation object is initialized (in the constructor).
        /// </summary>
        void DebugRegister();
    }
}
