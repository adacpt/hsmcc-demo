using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharpPro;

namespace SignagePlayerDevice
{
    public interface ISignagePlayer
    {
        void UpdateClassification(int classification);
        void UpdateMicMuteStatus(int micMute);
        void UpdateSpeakerMuteStatus(int speakerMute);
        void UpdateVTCActiveStatus(int vtcActive);
        void UpdateZoneName(String zoneName, String activeZone);
        void UpdateZoneHourOffset(int zoneHour, String activeZone);
        void UpdateZoneMinuteOffset(int zoneMinute, String activeZone);
        void ConnectRawSocket(string name, string ipAddress);
        void ConnectSerial(string name, ComPort serialPort, string baudRate);
        void Reconnect();


        event EventHandler<SignagePlayerEventArgs> SignagePlayerDeviceChangeEvent;
        event EventHandler<EventArgs> DeviceConnectionEvent;
        event EventHandler<EventArgs> DeviceDisconnectionEvent;
    }
}
