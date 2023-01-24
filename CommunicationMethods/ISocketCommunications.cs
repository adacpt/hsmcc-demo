using System;
using System.Net;

namespace CommunicationMethods
{
    /// <summary>
    /// Defines the behavior common across socket communications (TCP client, TCP server, SSH, etc). Implements <see cref="ISendReceiveAsync"/>.
    /// </summary>
    public interface ISocketCommunications : ISendReceiveAsync
    {
        event EventHandler DeviceConnected;

        event EventHandler DeviceDisconnected;

        /// <summary>
        /// Gets a value indicating whether the socket is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the <see cref="IPEndPoint"/> to which the socket will connect.
        /// </summary>
        /// <remarks>Should be set in constructors.</remarks>
        IPEndPoint EndPoint { get; }
    }
}