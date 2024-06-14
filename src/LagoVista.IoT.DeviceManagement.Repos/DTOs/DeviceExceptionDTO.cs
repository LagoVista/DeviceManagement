using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using NLog.LayoutRenderers.Wrappers;
using System;

namespace LagoVista.IoT.DeviceManagement.Repos.DTOs
{
    public class DeviceExceptionDTO : TableStorageEntity
    {
        public DeviceExceptionDTO(DeviceException exception, bool cleared)
        {
            RowKey = DateTime.UtcNow.ToInverseTicksRowKey();
            PartitionKey = exception.DeviceUniqueId;

            DeviceId = exception.DeviceId;            
            DeviceUniqueId = exception.DeviceUniqueId;            
            DeviceRepositoryId = exception.DeviceRepositoryId;
            Timestamp = exception.Timestamp;
            ErrorCode = exception.ErrorCode;
            Details = exception.Details;
            Cleared = cleared;
            Event = cleared ? "Cleared" : "Raised";
        }

        public DeviceExceptionDTO()
        {
        }

        public string DeviceUniqueId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceRepositoryId { get; set; }
        public string ErrorCode { get; set; }
        public string Details { get; set; }
        public string Timestamp { get; set; }

        public bool Cleared { get; set; }
        public string Event { get; set; }

        public DeviceException ToDeviceException()
        {
            return new DeviceException()
            {
                Details = Details,
                ErrorCode = ErrorCode,
                DeviceId = DeviceId,
                DeviceUniqueId = DeviceUniqueId,
                DeviceRepositoryId = DeviceRepositoryId,
                Timestamp = Timestamp
            };
        }
    }
}
