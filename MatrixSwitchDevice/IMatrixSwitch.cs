using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharpPro;

namespace MatrixSwitchDevice
{
    public interface IMatrixSwitch
    {
        void MakeRoute(int inputIndex, int outputIndex);
        void DisconnectSource(int inputIndex);
        void DisconnectDestination(int outputIndex);
        void ConnectRawSocket(string name, string ipAddress, int deviceID);
        void ConnectSerial(string name, ComPort serialPort, string baudRate);
        void Reconnect();

        event EventHandler<EventArgs> DeviceConnectionEvent;
        event EventHandler<EventArgs> DeviceDisconnectionEvent;
    }
}
