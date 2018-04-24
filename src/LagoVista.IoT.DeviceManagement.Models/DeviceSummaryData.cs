using LagoVista.Core.Models;
using LagoVista.Core.Models.Geo;
using LagoVista.IoT.DeviceManagement.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public class DeviceSummaryData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string LastContact { get; set; }

        public EntityHeader DeviceType { get; set; }

        public EntityHeader DeviceConfiguration { get; set; }

        public EntityHeader DeviceRepository { get; set; }

        public List<AttributeValue> Properties { get; set; }
        public List<AttributeValue> States { get; set; }
        public List<AttributeValue> Attributes { get; set; }

        public double Speed { get; set; }

        public double Headig { get; set; }
        public GeoLocation GeoLocation { get; set; }

        public EntityHeader Status { get; set; }


        public static DeviceSummaryData FromDevice(Device device)
        {
            return new DeviceSummaryData()
            {
                Attributes = device.Attributes,
                DeviceConfiguration = device.DeviceConfiguration,
                DeviceId = device.DeviceId,
                DeviceType = device.DeviceType,
                GeoLocation = device.GeoLocation,
                Headig = device.Heading,
                Id = device.Id,
                LastContact = device.LastContact,
                Properties = device.Properties,
                Speed = device.Speeed,
                DeviceName = device.Name,
                States = device.States,
                Status = device.Status
            };
        }
    }
}
