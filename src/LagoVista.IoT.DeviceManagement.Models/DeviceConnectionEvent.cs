// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b9f4b0ddd82cbbf6f282af55a99e2e8465819646d7ba4f2929e25ca223b6ac20
// IndexVersion: 0
// --- END CODE INDEX META ---
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
