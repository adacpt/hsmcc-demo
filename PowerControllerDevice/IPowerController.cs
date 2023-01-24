using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharpPro;

namespace PowerControllerDevice
{
    public interface IPowerController
    {
        void SetOutletPowerState(int outlet, int powerState);
        void GetOutletPowerState(int outlet);
        void ConnectRawSocket(string name, string ipAddress);
        void ConnectSerial(string name, ComPort serialPort, string baudRate);
        void Reconnect();

        event EventHandler<PowerControllerEventArgs> PowerControllerDeviceChangeEvent;
        event EventHandler<EventArgs> DeviceConnectionEvent;
        event EventHandler<EventArgs> DeviceDisconnectionEvent;
    }
}
