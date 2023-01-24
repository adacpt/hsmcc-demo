using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UserInteraction;
using MediaTransport;
using DevicesControlled;
using MAVETesting;



namespace Physical
{

    public interface IPhysical
    {
        
        void GetPillarConfigValues(String config);
        void InitializePhysicalPillarValues();
        void OnUserInteractionChangeMomentaryEvent(object sender, TouchPanelEventArgs args);
        void OnMediaTransportChangeEvent(object sender, MediaTransportEventArgs args);
        void OnDevicesControlledChangeEvent(object sender, DevicesControlledEventArgs args);

        event EventHandler<PhysicalEventArgs> PhysicalChangeEvent;

    }
}
