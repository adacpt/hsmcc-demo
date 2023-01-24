using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharpPro;
//using DevicesControlled;


namespace VtcDevice
{
    public interface IVtc
    {
        void MakeCall ( String number );
        void HangupCall ( String callName );
        void SetPresentation ( int presentation );
        void GetPresentation ();
        void SetSelfview( int selfview );
        void GetSelfview  ();
        void MovePipScreen ();
        void SetDtmf ( int dtmf );
        void GetDtmf ();
        void StoreNumber(int slot, String number);
        void StoreName(int slot, String name);
        void ConnectRawSocket(string name, string ipAddress, int deviceID);
        void ConnectSerial(string name, ComPort serialPort, string baudRate);
        void Reconnect();

        event EventHandler<EventArgs> DeviceConnectionEvent;
        event EventHandler<EventArgs> DeviceDisconnectionEvent;
    }
}
