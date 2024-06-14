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
            this.CurrentStatus = status.CurrentStatus;
            this.PreviouStatus = status.PreviouStatus;
            this.Details = status.Details;
            this.LastNotified = status.LastNotified;
            this.LastContact = status.LastContact;
            this.WatchdogCheckPoint = status.WatchdogCheckPoint;
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
                CurrentStatus = this.CurrentStatus,                
                WatchdogCheckPoint = this.WatchdogCheckPoint,
                LastContact = this.LastContact,
                LastNotified = this.LastNotified,
                Details = this.Details,
            };
        }

        public string DeviceUniqueId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceRepositoryId { get; set; }
        public string Timestamp { get; set; }
        public string LastContact { get; set; }
        public string LastNotified { get; set; }
        public string PreviouStatus { get; set; }
        public string CurrentStatus { get; set; }
        public string Details { get; set; }
        public string WatchdogCheckPoint { get; set; }
    }
}
