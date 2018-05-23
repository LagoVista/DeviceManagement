﻿using System.Collections.Generic;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public class DeviceArchiveResult
    {
        public DeviceArchiveResult()
        {
            Fields = new Dictionary<string, object>();
        }

        public string Timestamp { get; set; }
        public string PEMMessageId { get; set; }
        public Dictionary<string, object> Fields { get; set; }
    }
}
