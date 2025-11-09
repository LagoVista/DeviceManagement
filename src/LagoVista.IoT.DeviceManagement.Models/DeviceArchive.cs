// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 71e1da72fe9a83e5e19f63e20b1da58514cd898e51fde7ee9ad0367e80d3db5b
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public class DeviceArchive : TableStorageEntity
    {
        public DeviceArchive()
        {
            Properties = new Dictionary<string, object>();
        }

        /* Device ID is the ID associated with the device by the user, it generally will be unique, but can't assume it to be, it is read only */
        public string DeviceId { get; set; }

        public string MessageId { get; set; }

        public string DeviceConfigurationId { get; set; }

        public string Timestamp { get; set; }

        public string PEMMessageId { get; set; }

        /* Will be made available to map to properties on device configurations */
        public double DeviceConfigurationVersionId { get; set; }

        [JsonExtensionData]
        public Dictionary<String, object> Properties { get; set; }

    }
}
