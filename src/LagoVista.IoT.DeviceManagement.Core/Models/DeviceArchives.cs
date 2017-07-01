using LagoVista.Core.Models;
using LagoVista.IoT.DeviceAdmin.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public class DeviceArchive : TableStorageEntity
    {
        public DeviceArchive()
        {
            Properties = new Dictionary<string, object>();
        }

        /* Device ID is the ID associated with the device by the user, it generally will be unique, but can't assume it to be, it's primarily read only */
        public string DeviceId { get; set; }

        public string DeviceConfigurationId { get; set; }

        public string Timestamp { get; set; }

        public string PEMMessageId { get; set; }

        /* Will be made available to map to properties on device configurations */
        public double DeviceConfigurationVersionId { get; set; }

        [JsonExtensionData]
        public Dictionary<String, object> Properties { get; set; }

    }
}
