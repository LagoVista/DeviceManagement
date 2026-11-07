using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;

namespace LagoVista.IoT.DeviceManagement.Repos.DTOs
{
    public sealed class DeviceConnectionActivityRecord : IActivityRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public System.DateTime CreationDate { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceId { get; set; }
        public string Timestamp { get; set; }
        public string FirmwareSKU { get; set; }
        public string FirmwareRevision { get; set; }
        public double RSSI { get; set; }
        public bool Reconnect { get; set; }

        public static DeviceConnectionActivityRecord From(DeviceRepository repo, DeviceConnectionEvent connectionEvent) => new DeviceConnectionActivityRecord
        {
            Id = RuntimeDataRecordMapper.NewId(),
            OrganizationId = repo.Id,
            Organization = repo.Name,
            CreationDate = RuntimeDataRecordMapper.ParseTimestamp(connectionEvent.TimeStamp),
            DeviceUniqueId = connectionEvent.DeviceId,
            DeviceId = connectionEvent.DeviceId,
            Timestamp = connectionEvent.TimeStamp,
            FirmwareSKU = connectionEvent.FirmwareSKU,
            FirmwareRevision = connectionEvent.FirmwareRevision,
            RSSI = connectionEvent.RSSI,
            Reconnect = connectionEvent.Reconnect
        };

        public DeviceConnectionEvent ToModel() => new DeviceConnectionEvent
        {
            DeviceId = DeviceId,
            TimeStamp = Timestamp,
            FirmwareSKU = FirmwareSKU,
            FirmwareRevision = FirmwareRevision,
            RSSI = RSSI,
            Reconnect = Reconnect
        };
    }
}
