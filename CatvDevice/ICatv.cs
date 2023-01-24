using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharpPro;

namespace CatvDevice
{
    public interface ICatv
    {
        void ChannelUp();
        void ChannelDown();
        void SetChannel(String channel);
        void PreviousChannel();
        void GetChannel();
        void MenuUp();
        void MenuDown();
        void MenuLeft();
        void MenuRight();
        void MenuEnter();
        void Menu();
        void Exit();
        void List();
        void Info();
        void SavePreset(int preset);
        void RecallPreset(int preset);
        void GetPresets();
        void ConnectRawSocket(string name, string ipAddress, int deviceID);
        void ConnectSerial(string name, ComPort serialPort, string baudRate);
        void Reconnect();
        
        event EventHandler<CatvEventArgs> CatvChangeEvent;
        event EventHandler<EventArgs> DeviceConnectionEvent;
        event EventHandler<EventArgs> DeviceDisconnectionEvent;
    }
}
