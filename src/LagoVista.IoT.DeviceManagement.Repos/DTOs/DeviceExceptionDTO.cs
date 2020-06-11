using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System;

namespace LagoVista.IoT.DeviceManagement.Repos.DTOs
{
    public class DeviceExceptionDTO : TableStorageEntity
    {
        public DeviceExceptionDTO(DeviceException exception)
        {
            PartitionKey = exception.DeviceId;
            DeviceId = exception.DeviceId;
            DeviceRepositoryId = exception.DeviceRepositoryId;
            Timestamp = exception.Timestamp;
            ErrorCode = exception.ErrorCode;
            Details = exception.Details;

            RowKey = DateTime.UtcNow.ToInverseTicksRowKey();
        }

        public DeviceExceptionDTO()
        {
        }

        public string DeviceId { get; set; }
        public string DeviceRepositoryId { get; set; }
        public string ErrorCode { get; set; }
        public string Details { get; set; }
        public string Timestamp { get; set; }

        public DeviceException ToDeviceException()
        {
            return new DeviceException()
            {
                Details = Details,
                ErrorCode = ErrorCode,
                DeviceId = RowKey,
                DeviceRepositoryId = DeviceRepositoryId,
                Timestamp = Timestamp
            };
        }
    }
}
