using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System;

namespace LagoVista.IoT.DeviceManagement.Repos.DTOs
{
    public class DeviceStatusDTO : TableStorageEntity
    {
        public DeviceStatusDTO(DeviceStatus status)
        {
            RowKey = DateTime.UtcNow.ToInverseTicksRowKey();

            this.DeviceUniqueId = status.DeviceId;
            PartitionKey = status.DeviceId;

            this.NewStatus = status.NewStatus;
            this.PreviouStatus = status.PreviouStatus;
            this.Details = status.Details;
        }

        public DeviceStatusDTO()
        {
        }

        public DeviceStatus ToDeviceStatus()
        {
            return new DeviceStatus()
            {
                DeviceId = this.DeviceUniqueId,
                DeviceUniqueId = this.DeviceUniqueId,
                Timestamp = this.Timestamp,
                PreviouStatus = this.PreviouStatus,
                NewStatus = this.NewStatus,
                Details = this.Details
            };
        }

        public string DeviceUniqueId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceRepositoryId { get; set; }
        public string Timestamp { get; set; }
        public string PreviouStatus { get; set; }
        public string NewStatus { get; set; }
        public string Details { get; set; }
    }
}
