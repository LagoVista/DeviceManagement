// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 23dc291dc1712ca7054189d2cbc2b1bf92bd23535a91816dba5f2f7053624987
// IndexVersion: 2
// --- END CODE INDEX META ---
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
