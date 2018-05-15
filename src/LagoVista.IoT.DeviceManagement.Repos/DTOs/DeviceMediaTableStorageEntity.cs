using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System;
using LagoVista.Core;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Repos
{
    public class DeviceMediaTableStorageEntity : TableStorageEntity
    {
        public string DeviceId { get; set; }
        public string TimeStamp { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }

        public DeviceMedia ToDeviceMedia()
        {
            return new DeviceMedia()
            {
                DeviceId = DeviceId,
                ContentType = ContentType,
                FileName = FileName,
                Title = Title,
                TimeStamp = TimeStamp,
                ItemId = RowKey
            };
        }

        public static DeviceMediaTableStorageEntity FromDeviceMedia(DeviceMedia item)
        {
            return new DeviceMediaTableStorageEntity()
            {
                DeviceId = item.DeviceId,
                RowKey = DateTime.UtcNow.ToInverseTicksRowKey().Replace(".","-"),
                PartitionKey = item.DeviceId,
                ContentType = item.ContentType,
                TimeStamp = item.TimeStamp,
                FileName = item.FileName,
                Title = item.Title,
            };
        }
    }
}
