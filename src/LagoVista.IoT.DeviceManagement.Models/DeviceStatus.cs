using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class DeviceStatus
    {
        public string DeviceId { get; set; }
        public string DeviceUniqueId { get; set; }
        public string Timestamp { get; set; }
        public string PreviouStatus { get; set; }
        public string NewStatus { get; set; }
        public string Details { get; set; }
    }
}
