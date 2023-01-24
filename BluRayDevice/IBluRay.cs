using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharpPro;

namespace BluRayDevice
{
    public interface IBluRay
    {
        
        void Play();
        void Pause();
        void Stop();
        void NextChapter();
        void LastChapter();
        void FastForward();
        void Rewind();
        void Number(int number);
        void MenuUp();
        void MenuDown();
        void MenuLeft();
        void MenuRight();
        void MenuEnter();
        void Home();
        void Setup();
        void Options();
        void Info();
        void Return();
        void PopupMenu();
        void TopMenu();
        void SpecialKey(int key);
        void ConnectRawSocket(string name, string ipAddress);
        void ConnectSerial(string name, ComPort serialPort, string baudRate);
        void Reconnect();

        event EventHandler<BluRayEventArgs> BluRayDeviceChangeEvent;
        event EventHandler<EventArgs> DeviceConnectionEvent;
        event EventHandler<EventArgs> DeviceDisconnectionEvent;
    }
}
