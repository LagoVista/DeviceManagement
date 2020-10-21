using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class DeviceConnectionEvent
    {
        public string DeviceId { get; set; }
        public string TimeStamp { get; set; }
        public string FirmwareSKU { get; set; }
        public string FirmwareRevision { get; set; }
        public double RSSI { get; set; }
        public bool Reconnect { get; set; }
    }
}
