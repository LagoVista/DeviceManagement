using LagoVista.Core.Models;
using Newtonsoft.Json;
using System;
using LagoVista.Core;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public class DeviceGroupEntry
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public EntityHeader DeviceType { get; set; }
        public EntityHeader DeviceConfiguration { get; set; }

        public string DateAdded { get; set; }
        public EntityHeader AddedBy { get; set; }

        public static DeviceGroupEntry FromDevice(Device device, EntityHeader addedBy)
        {
            return new DeviceGroupEntry()
            {
                Id = Guid.NewGuid().ToId(),
                DateAdded = DateTime.UtcNow.ToJSONString(),
                AddedBy = addedBy,
                DeviceUniqueId = device.Id,
                DeviceId = device.DeviceId,
                Name = device.Name,
                DeviceType = device.DeviceType,
                DeviceConfiguration = device.DeviceConfiguration
            };
        }
    }
}
