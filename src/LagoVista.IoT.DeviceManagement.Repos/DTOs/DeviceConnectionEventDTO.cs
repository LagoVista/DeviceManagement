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
