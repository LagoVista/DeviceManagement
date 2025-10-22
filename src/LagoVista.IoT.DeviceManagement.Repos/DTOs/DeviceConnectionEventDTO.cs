// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 614aa06b93e7b9c8b8dc25149bbde53d708dfd69c27765e7d660b32ef40eb172
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Repos.DTOs
{
    public class DeviceConnectionEventDTO : TableStorageEntity
    {
        public DeviceConnectionEventDTO()
        {

        }

        public DeviceConnectionEventDTO(DeviceConnectionEvent connectionEvent, bool cleared)
        {
            RowKey = DateTime.UtcNow.ToInverseTicksRowKey();
            PartitionKey = connectionEvent.DeviceId;
            DeviceId = connectionEvent.DeviceId;
            TimeStamp = connectionEvent.TimeStamp;
            FirmwareSKU = connectionEvent.FirmwareSKU;
            FirmwareRevision = connectionEvent.FirmwareRevision;
            RSSI = connectionEvent.RSSI;
        }

        public DeviceConnectionEvent ToDeviceConnectionEvent()
        {
            return new DeviceConnectionEvent()
            {
                DeviceId = DeviceId,
                TimeStamp = TimeStamp,
                FirmwareSKU = FirmwareSKU,
                FirmwareRevision = FirmwareRevision,
                RSSI = RSSI,
            };
        }

        public string DeviceId { get; set; }
        public string TimeStamp { get; set; }
        public string FirmwareSKU { get; set; }
        public string FirmwareRevision { get; set; }
        public double RSSI { get; set; }
    }
}
