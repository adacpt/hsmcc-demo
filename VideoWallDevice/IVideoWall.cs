using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharpPro;

namespace VideoWallDevice
{
    public interface IVideoWall
    {
        void SetLayout(int layout);
        void GetLayout();
        void ConnectRawSocket(string name, string ipAddress, int deviceID);
        void ConnectSerial(string name, ComPort serialPort, string baudRate);
        void Reconnect();

        event EventHandler<VideoWallEventArgs> VideoWallDeviceChangeEvent;
        event EventHandler<EventArgs> DeviceConnectionEvent;
        event EventHandler<EventArgs> DeviceDisconnectionEvent;
    }
}
