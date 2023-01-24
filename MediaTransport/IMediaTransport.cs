using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAVETesting;
using UserInteraction;
using DevicesControlled;
using Physical;

namespace MediaTransport
{
    public interface IMediaTransport
    {
        void GetPillarConfigValues(String config);
        void InitializeMediaTransportValues();
        void OnUserInteractionChangeMomentaryEvent(object sender, TouchPanelEventArgs args);
        void OnUserInteractionChangeHoldEvent(object sender, TouchPanelEventArgs args);
        void OnPhysicalChangeEvent(object sender, PhysicalEventArgs e);
        void OnDevicesControlledChangeEvent(object sender, DevicesControlledEventArgs args);
    }
}
