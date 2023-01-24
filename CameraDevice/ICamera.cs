using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharpPro;

namespace CameraDevice
{
    public interface ICamera
    {
        event EventHandler<CameraEventArgs> PresetSavedEvent;

        event EventHandler<CameraEventArgs> PresetRecallEvent;

        void Pan ( int direction, int speed, int action, string controlName);
        void Tilt ( int direction, int speed, int action, string controlName);
        void Zoom ( int direction, int speed, int action, string controlName);
        void Stop(string controlName);
        void RecallPreset ( int preset, string controlName);
        void SavePreset (int preset, string controlName);
        void Power(int power, string controlName);
        void Home(string controlName);
        void RecallPosition(int pan, int tilt, int zoom);
        void ConnectRawSocket( string name, string ipAddress );
        void ConnectSerial(string name, ComPort serialPort, string baudRate);
    }
}
