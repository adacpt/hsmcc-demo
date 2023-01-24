using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharpPro;

namespace DspDevice
{
    public interface IDsp
    {
        void AdjustVolume( String level, int volume );
        void SetVolume( String level, int volume );
        void GetVolume( String level );
        void ToggleVolumeMute(String level);
        void SetVolumeMute( String level, int mute );
        void GetVolumeMute( String level );
        void ToggleMicMute(String level);
        void SetMicMute( String level, int mute );
        void GetMicMute( String level );
        void MakeCall(String number);
        void HangupCall();
        void StoreNumber(int slot, String number);
        void StoreName(int slot, String name);
        void ConnectRawSocket(string name, string ipAddress, int deviceID);
        void ConnectSerial(string name, ComPort serialPort, string baudRate);
        void Reconnect();

        event EventHandler<DspEventArgs> DspChangeEvent;
        event EventHandler<EventArgs> DeviceConnectionEvent;
        event EventHandler<EventArgs> DeviceDisconnectionEvent;
    }
}
