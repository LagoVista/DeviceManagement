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
            Properties = new List<CustomField>();
        }

        /* Partition Key will be the give id */

        /* Device ID is the ID associated with the device by the user, it generally will be unique, but can't assume it to be, it's primarily read only */
        public string DeviceId { get; set; }

        public string DeviceConfigurationId { get; set; }

        public string DateStamp { get; set; }

        public string PEMMessageId { get; set; }

        /* Will be made available to map to properties on device configurations */
        public double DeviceConfigurationVersionId { get; set; }

        /* Will be smart and dynamically add properties that get serialized with Table Storage */
        /* Will then be smart and construct a deserialized json package with the properties appropriate for this device */


        public String PropertiesJSON
        {
            get { return JsonConvert.SerializeObject(Properties);  }
            set { Properties = JsonConvert.DeserializeObject<List<CustomField>>(value); }
        }

        [JsonIgnore]
        public List<CustomField> Properties { get; set; }

    }
}
