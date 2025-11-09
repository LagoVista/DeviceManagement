// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0b3d6f5b87b4162221230d53158a74f38f60c49bde88d7e88514120e65dda7bf
// IndexVersion: 2
// --- END CODE INDEX META ---
using System.Collections.Generic;

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
