using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharpPro;

namespace DisplayDevice
{
    public interface IDisplay
    {
        void SetPower(int power);
        void GetPower();
        void SetInput(int input);
        void GetInput();
        void AdjustVolume(int volume);
        void SetVolume(int volume);
        void GetVolume();
        void SetVolumeMute(int mute);
        void GetVolumeMute();
        void SetMultiView(int multiview);
        void GetMultiView();
        void ConnectRawSocket(string name, string ipAddress, int deviceID);
        void ConnectSerial(string name, ComPort serialPort, string baudRate);
        void Reconnect();

        event EventHandler<DisplayEventArgs> DisplayChangeEvent;
        event EventHandler<EventArgs> DeviceConnectionEvent;
        event EventHandler<EventArgs> DeviceDisconnectionEvent;

    }
}
