using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;

namespace LagoVista.IoT.DeviceManagement.Repos.DTOs
{
    public sealed class DeviceProximityObservationActivityRecord : IActivityRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public DateTime CreationDate { get; set; }
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

        public static DeviceProximityObservationActivityRecord From(DeviceProximityObservation source) => new DeviceProximityObservationActivityRecord
        {
            Id = String.IsNullOrWhiteSpace(source.Id) ? Guid.NewGuid().ToString("N") : source.Id,
            OrganizationId = source.OrganizationId,
            Organization = source.Organization,
            CreationDate = source.TimeStamp,
            DeviceRepoId = source.DeviceRepoId,
            DeviceUniqueId = source.DeviceUniqueId,
            DeviceName = source.DeviceName,
            DeviceId = source.DeviceId,
            DeviceType = source.DeviceType,
            DeviceTypeId = source.DeviceTypeId,
            ObservedDeviceId = source.ObservedDeviceId,
            Rssi = source.Rssi,
            DistanceEstimate = source.DistanceEstimate,
            SourceMessageType = source.SourceMessageType
        };

        public DeviceProximityObservation ToModel() => new DeviceProximityObservation
        {
            Id = Id,
            OrganizationId = OrganizationId,
            Organization = Organization,
            TimeStamp = CreationDate,
            DeviceRepoId = DeviceRepoId,
            DeviceUniqueId = DeviceUniqueId,
            DeviceName = DeviceName,
            DeviceId = DeviceId,
            DeviceType = DeviceType,
            DeviceTypeId = DeviceTypeId,
            ObservedDeviceId = ObservedDeviceId,
            Rssi = Rssi,
            DistanceEstimate = DistanceEstimate,
            SourceMessageType = SourceMessageType
        };
    }
}
