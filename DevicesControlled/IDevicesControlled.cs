using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAVETesting;
using UserInteraction;
using Physical;
using MediaTransport;

namespace DevicesControlled
{
    public interface IDevicesControlled
    {
        void GetPillarConfigValues(String config);
        void InitializeDevicesControlledValues();
        void OnUserInteractionChangeMomentaryEvent(object sender, TouchPanelEventArgs args);
        void OnUserInteractionChangeHoldEvent(object sender, TouchPanelEventArgs args);
        void OnPhysicalChangeEvent(object sender, PhysicalEventArgs e);
        void OnDevicesControlledChangeEvent(object sender, DevicesControlledEventArgs args);

        void OnMediaTransportChangeEvent(object sender, MediaTransportEventArgs args);
    }
}
