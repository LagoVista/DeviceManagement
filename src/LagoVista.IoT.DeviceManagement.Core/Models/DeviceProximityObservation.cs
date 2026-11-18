using System;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    /// <summary>One proximity reading reported by a registered device.</summary>
    public class DeviceProximityObservation
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public DateTime TimeStamp { get; set; }
        public string DeviceRepoId { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string DeviceTypeId { get; set; }
        public string ObservedDeviceId { get; set; }
        public double Rssi { get; set; }
        public double DistanceEstimate { get; set; }
        public string SourceMessageType { get; set; }
    }
}
