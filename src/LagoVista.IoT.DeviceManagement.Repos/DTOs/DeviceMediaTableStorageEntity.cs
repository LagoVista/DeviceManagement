using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System;
using LagoVista.Core;

namespace LagoVista.IoT.DeviceManagement.Repos
{
    public class DeviceMediaTableStorageEntity : TableStorageEntity
    {
        public string DeviceId { get; set; }
        public string TimeStamp { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }


        public DeviceMedia ToDeviceMedia()
        {
            return new DeviceMedia()
            {
                DeviceId = DeviceId,
                ContentType = ContentType,
                FileName = FileName,
                Title = Title,
                TimeStamp = TimeStamp,
                ItemId = RowKey,
                Location = (Latitude.HasValue && Longitude.HasValue) ? new LagoVista.Core.Models.Geo.GeoLocation(Latitude.Value, Longitude.Value) : null
               
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
                Latitude = item.Location != null ? item.Location.Latitude : null,
                Longitude = item.Location != null ? item.Location.Longitude : null,
            };
        }
    }
}
