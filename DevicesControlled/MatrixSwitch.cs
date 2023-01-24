using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatrixSwitchDevice;

namespace DevicesControlled
{
    public class MatrixSwitch
    {
        public String Manufacturer { get; set; }
        public IMatrixSwitch MatrixSwitchImpl { get; set; }
        public EConnectionType ConnectionType { get; set; }
        public RawSocketConnectionInfo RawSocketConnectionInfo { get; set; }
        public SerialConnectionInfo SerialConnectionInfo { get; set; }
    }
}
