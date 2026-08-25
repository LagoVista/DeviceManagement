using LagoVista.Core.Interfaces;
using System;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class DeviceConnectionEvent : IActivityRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public DateTime CreationDate { get; set; }

        public string DeviceId { get; set; }
        public string TimeStamp { get; set; }
        public string FirmwareSKU { get; set; }
        public string FirmwareRevision { get; set; }
        public double RSSI { get; set; }
        public bool Reconnect { get; set; }
    }
}
