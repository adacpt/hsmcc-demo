using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using Diagnostics.Debugging;
using Diagnostics.Logging;

namespace CommunicationMethods
{
    /// <summary>
    /// Class for TCP/IP socket client communications. Implements <see cref="ISocketCommunications"/>.
    /// </summary>
    public class TcpClient : ISocketCommunications
    {
        private bool enable;
        private Socket socket;
        private System.Timers.Timer pollingTimer = new System.Timers.Timer(1000)
        {
            AutoReset = true,
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClient"/> class.
        /// </summary>
        /// <param name="name">Name for debugging.</param>
        /// <param name="ipAddress">IP address of device.</param>
        /// <param name="port">Port number for device.</param>
        public TcpClient(string name, string ipAddress, int port)
        {
            try
            {
                EndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                Name = name;
                pollingTimer.Elapsed += PollingTimer_Elapsed;
                DebugRegister();
            }
            catch (Exception e)
            {
                LogManager.CreateLogEntry(new LogEntry(
                    string.Empty,
                    string.Empty,
                    this.Name,
                    $"Error in constructor: {e.Message}",
                    SeverityLevel.Error));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClient"/> class.
        /// </summary>
        /// <param name="endPoint">IP endpoint to connect to.</param>
        public TcpClient(IPEndPoint endPoint)
        {
            try
            {
                EndPoint = endPoint;
                pollingTimer.Elapsed += PollingTimer_Elapsed;
                DebugRegister();
            }
            catch (Exception e)
            {
                LogManager.CreateLogEntry(new LogEntry(
                    string.Empty,
                    string.Empty,
                    this.Name,
                    $"Error in constructor: {e.Message}",
                    SeverityLevel.Error));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClient"/> class.
        /// </summary>
        /// <param name="hostname">Hostname that DNS will lookup.</param>
        /// <param name="port">Port number to use for the connection.</param>
        public TcpClient(string hostname, int port)
            : this(GetEndpointFromHostname(hostname, port))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClient"/> class.
        /// </summary>
        /// <param name="ipAddress">The IP address of the device.</param>
        /// <param name="port">Port number to use for the connection.</param>
        public TcpClient(IPAddress ipAddress, int port)
            : this(new IPEndPoint(ipAddress, port))
        {
        }

        /// <inheritdoc/>
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <inheritdoc/>
        public event EventHandler<string> DebugDataSent;

        /// <inheritdoc/>
        public event EventHandler<string> DebugDataReceived;

        /// <inheritdoc/>
        public event EventHandler DeviceConnected;

        /// <inheritdoc/>
        public event EventHandler DeviceDisconnected;

        /// <summary>
        /// Gets the IP endpoint where the socket connection will be made. Can only be set at constructor.
        /// </summary>
        public IPEndPoint EndPoint { get; private set; }

        /// <summary>
        /// Gets the data receive buffer size in bytes. Default value is 1000, can only be changed during instantiation.
        /// </summary>
        public int BufferSize { get; } = 1000;

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Gets or sets the timeout interval for connection attempts (in milliseconds).
        /// </summary>
        /// <value>Units in milliseconds. Default is 5000 ms.</value>
        public int TimeoutInterval { get; set; } = 5000;

        /// <inheritdoc/>
        public bool Enable
        {
            get
            {
                return enable;
            }

            set
            {
                // first make sure the new value is actually different
                if (value != enable)
                {
                    enable = value;

                    if (enable)
                    {
                        Connect();
                    }
                    else
                    {
                        if (socket != null)
                        {
                            socket.Close();
                            socket.Dispose();
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public bool EnableDebug { get; set; }

        /// <summary>
        /// Attempts to retrieve an IP address from a hostname using DNS. If multiple are found, this is logged and the first entry is returned.
        /// If none are found, returns loopback address.
        /// </summary>
        /// <param name="hostname">Host name to lookup in DNS.</param>
        /// <param name="port">Port number to use on IP endpoint.</param>
        /// <returns>IP endpoint found using DNS.</returns>
        public static IPEndPoint GetEndpointFromHostname(string hostname, int port)
        {
            var addresses = Dns.GetHostAddresses(hostname);
            if (addresses.Length == 0)
            {
                LogManager.CreateLogEntry(string.Empty, string.Empty, hostname, $"DNS unable to retrieve address from specified hostname: {hostname}. Using loopback address.", SeverityLevel.Error);
                return new IPEndPoint(IPAddress.Loopback, port);
            }
            else if (addresses.Length > 1)
            {
                LogManager.CreateLogEntry(string.Empty, string.Empty, hostname, $"DNS found multiple IP addresses for the specified hostname: {hostname}", SeverityLevel.Warning);
            }

            return new IPEndPoint(addresses[0], port);
        }

        /// <inheritdoc/>
        public void DebugRegister()
        {
            DebugEnvironment.DebuggableObjects.Add(this);
        }

        /// <inheritdoc/>
        public void OnDebugDataReceived(string data)
        {
            DebugDataReceived?.Invoke(this, data);
        }

        /// <inheritdoc/>
        public void OnDebugDataSent(string data)
        {
            DebugDataSent?.Invoke(this, data);
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
            if (EnableDebug) { DebugEnvironment.Write($"Tx: {data}"); }
            SendRaw(Encoding.UTF8.GetBytes(data));
        }

        /// <inheritdoc/>
        public void SendBytes(byte[] data)
        {
            if (EnableDebug) { DebugEnvironment.Write($"Tx: {BitConverter.ToString(data)}"); }
            SendRaw(data);
        }

        private void SendRaw(byte[] data)
        {
            if (socket != null)
            {
                if (socket.Connected)
                {
                    Thread sendThread = new Thread(() =>
                        socket.Send(data));
                    sendThread.Start();
                }
                else
                {
                    if (EnableDebug) { DebugEnvironment.Write("Cannot send, socket not connected"); }
                }
            }
            else
            {
                if (EnableDebug) { DebugEnvironment.Write("Cannot send data to null socket"); }
            }
        }

        private void PollingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsSocketConnected(socket))
            {
                if (EnableDebug) { DebugEnvironment.Write("Socket disconnected"); }
                LogManager.CreateLogEntry(string.Empty, string.Empty, this.Name, "Socket disconnected", SeverityLevel.Warning);

                IsConnected = false;
                // ConnectionChanged?.Invoke(this, false);

                DeviceDisconnected?.Invoke(this, EventArgs.Empty);

                if (enable)
                {
                    if (EnableDebug) { DebugEnvironment.Write("Attempting auto-reconnect"); }
                    Connect();
                }

                pollingTimer.Stop();
            }
        }

        private void Connect()
        {
            if (EndPoint != null)
            {
                if (EnableDebug) { DebugEnvironment.Write("Attempting connection"); }

                // initialize the socket
                socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveTimeout = TimeoutInterval,
                    SendTimeout = TimeoutInterval,
                };

                // set up async callback
                SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs()
                {
                    RemoteEndPoint = EndPoint,
                };
                connectArgs.Completed += ConnectCompleted;

                // start the async connection attempt
                socket.ConnectAsync(connectArgs);
            }
        }

        private void ConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            // if this is not null then the connection was successful
            if (e.ConnectSocket != null)
            {
                if (e.ConnectSocket.Connected)
                {
                    socket = e.ConnectSocket;

                    StateObject state = new StateObject()
                    {
                        WorkSocket = socket,
                    };

                    // set up async receive
                    socket.BeginReceive(state.Buffer, 0, BufferSize, 0, new AsyncCallback(DataReceivedCallback), state);

                    // raise the connected event
                    IsConnected = true;
                    // ConnectionChanged?.Invoke(this, true); // TODO: change this to begininvoke

                    DeviceConnected?.Invoke(this, EventArgs.Empty);

                    if (EnableDebug) { DebugEnvironment.Write("Socket connected"); }
                    LogManager.CreateLogEntry(string.Empty, string.Empty, this.Name, "Socket connected", SeverityLevel.Notice);

                    // start polling
                    pollingTimer.Start();
                }
            }

            // if false, then the connection failed. Retry if still enabled
            else if (enable)
            {
                if (EnableDebug) { DebugEnvironment.Write("Connection attempt failed. Retrying..."); }
                Connect();
            }
        }

        private void DataReceivedCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket socket = state.WorkSocket;
                int bytesRead = socket.EndReceive(ar);

                // if we received bytes...
                if (bytesRead > 0)
                {
                    try
                    {
                        // trim the unused bytes
                        byte[] trimmedBuffer = TrimZeros(state.Buffer);

                        // set up a string variable for convertying byte[] to string
                        string receivedString = string.Empty;
                        try
                        {
                            // attempt to convert to string
                            receivedString = Encoding.ASCII.GetString(trimmedBuffer, 0, trimmedBuffer.Length);
                            if (EnableDebug) { DebugEnvironment.Write($"Rx: {receivedString}"); }
                        }

                        // catch is thrown if converting to a string fails
                        catch
                        {
                            if (EnableDebug) { DebugEnvironment.Write($"Rx: {BitConverter.ToString(trimmedBuffer)}"); }
                        }

                        // raise data received event
                        // TODO: change to begininvoke
                        DataReceived?.Invoke(this, new DataReceivedEventArgs()
                        {
                            DataBytes = trimmedBuffer,
                            DataString = receivedString,
                        });
                    }
                    catch (Exception e)
                    {
                        if (EnableDebug) { DebugEnvironment.Write($"Error processing received data: {e.Message}"); }
                    }
                }

                // start receiving data again
                Array.Clear(state.Buffer, 0, state.Buffer.Length);
                socket.BeginReceive(state.Buffer, 0, BufferSize, 0, new AsyncCallback(DataReceivedCallback), state);
            }
            catch (Exception e)
            {
                if (EnableDebug) { DebugEnvironment.Write($"Error in DataReceivedCallback: {e.Message}"); }
            }
        }

        private bool IsSocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = s.Available == 0;
            return !(part1 && part2);
        }

        private byte[] TrimZeros(byte[] packet)
        {
            var i = packet.Length - 1;
            while (packet[i] == 0)
            {
                --i;
            }

            var temp = new byte[i + 1];
            Array.Copy(packet, temp, i + 1);
            return temp;
        }

        private class StateObject
        {
            // Size of receive buffer.
            public const int BufferSize = 10000;

            // Client socket.
            public Socket WorkSocket = null;

            // Receive buffer.
            public byte[] Buffer = new byte[BufferSize];
        }
    }
}
