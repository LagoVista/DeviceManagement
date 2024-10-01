using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class SilencedAlarm
    {
        public SilencedAlarm()
        {

        }

        public bool Disabled { get; set; }
        public EntityHeader User { get; set; }
        public EntityHeader Device { get; set; }
        public EntityHeader DeviceRepo { get; set; }
        public string Timestamp { get; set; } 
        public string Details { get; set; }
    }
}
