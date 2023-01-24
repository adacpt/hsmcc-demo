using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAVETesting;
using MediaTransport;
using Physical;
using DevicesControlled;


namespace UserInteraction
{
    public interface IUserInteraction
    {
        
        void GetPillarConfigValues(String config);
        void InitializeUserInteractionValues();
        void OnMediaTransportChangeEvent(object sender, MediaTransportEventArgs args);
        void OnDevicesControlledChangeEvent(object sender, DevicesControlledEventArgs args);
        void OnPhysicalChangeEvent(object sender, Physical.PhysicalEventArgs e);
        void OnUserInteractionChangeHoldEvent(object sender, TouchPanelEventArgs args);
        void OnUserInteractionChangeMomentaryEvent(object sender, TouchPanelEventArgs args);
        
    }
}
