using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Diagnostics.Debugging;

namespace CommunicationMethods
{
    internal class SshClient : ISocketCommunications
    {
        /// <inheritdoc/>
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <inheritdoc/>
        public event EventHandler<string> DebugDataSent;

        /// <inheritdoc/>
        public event EventHandler<string> DebugDataReceived;

        public event EventHandler DeviceConnected;

        public event EventHandler DeviceDisconnected;

        /// <inheritdoc/>
        public bool IsConnected => throw new NotImplementedException();

        /// <inheritdoc/>
        public IPEndPoint EndPoint => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Enable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public bool EnableDebug { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public void DebugRegister()
        {
            DebugEnvironment.DebuggableObjects.Add(this);
        }

        /// <inheritdoc/>
        public void OnDebugDataSent(string data)
        {
            DebugDataSent?.Invoke(this, data);
        }

        /// <inheritdoc/>
        public void OnDebugDataReceived(string data)
        {
            DebugDataReceived?.Invoke(this, data);
        }

        /// <inheritdoc/>
        public void SendDataFromDebug(string data)
        {
            SendString(data);
        }

        /// <inheritdoc/>
        public void SendDataFromDebug(byte[] data)
        {
            SendBytes(data);
        }

        /// <inheritdoc/>
        public void SendString(string data)
        {
            // TODO
        }

        /// <inheritdoc/>
        public void SendBytes(byte[] data)
        {
            // TODO
        }
    }
}
