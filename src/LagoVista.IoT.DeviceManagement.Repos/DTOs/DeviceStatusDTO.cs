// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 609a52f1a497c4a801dc0555cd1ab8774bfc9a05a4f24db5fc92e7c1bb41ea1a
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System;

namespace LagoVista.IoT.DeviceManagement.Repos.DTOs
{
    public class DeviceStatusDTO : TableStorageEntity
    {
        public DeviceStatusDTO(DeviceStatus status, string rowKey)
        {
            RowKey = rowKey;

            this.DeviceId = status.DeviceId;
            PartitionKey = status.DeviceUniqueId;
            DeviceUniqueId = status.DeviceUniqueId;
            SilenceAlarm = status.SilenceAlarm;
            this.CurrentStatus = status.CurrentStatus;
            this.PreviousStatus = status.PreviousStatus;
         
            this.Details = status.Details;
            this.LastNotified = status.LastNotified;
            this.LastContact = status.LastContact;
            this.WatchdogCheckPoint = status.WatchdogCheckPoint;
            this.WatchdogTimeoutSeconds = status.WatchdogTimeoutSeconds;
        }

        public DeviceStatusDTO()
        {
        }

        public DeviceStatus ToDeviceStatus()
        {
            return new DeviceStatus()
            {
                DeviceId = this.DeviceId,
                DeviceUniqueId = this.DeviceUniqueId,
                Timestamp = this.Timestamp,
                PreviousStatus = this.PreviousStatus,
                CurrentStatus = this.CurrentStatus,                
                WatchdogCheckPoint = this.WatchdogCheckPoint,
                LastContact = this.LastContact,
                LastNotified = this.LastNotified,
                Details = this.Details,
                SilenceAlarm = this.SilenceAlarm,
                WatchdogTimeoutSeconds = this.WatchdogTimeoutSeconds,
            };
        }

        public int WatchdogTimeoutSeconds { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceRepositoryId { get; set; }
        public string Timestamp { get; set; }
        public string LastContact { get; set; }
        public string LastNotified { get; set; }
        public string PreviousStatus { get; set; }
        public string CurrentStatus { get; set; }
        public string Details { get; set; }
        public string WatchdogCheckPoint { get; set; }
        public bool SilenceAlarm { get; set; }
    }
}
