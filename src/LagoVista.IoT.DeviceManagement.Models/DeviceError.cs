using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class DeviceError
    {
        public string FirstSeen { get; set; }
        public int Count { get; set; }
        public string Timestamp { get; set; }
        public string Expires { get; set; }
        public string NextNotification { get; set; }
        public string DeviceErrorCode { get; set; }
    }
}
