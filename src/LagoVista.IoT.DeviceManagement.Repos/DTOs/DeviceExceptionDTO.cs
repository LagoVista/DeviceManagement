// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ec0fd0dd5660741b558f642f784423bb3990327482adf7a4b5c12b58e0b94ad8
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System;
using System.Linq;

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
            AdditionalDetails = exception.AdditionalDetails == null ? String.Empty : String.Join(',', exception.AdditionalDetails);
            Cleared = exception.Cleared;
            Event = exception.Event;
        }

        public DeviceExceptionDTO()
        {
        }

        public string DeviceUniqueId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceRepositoryId { get; set; }
        public string ErrorCode { get; set; }
        public string Details { get; set; }
        public string AdditionalDetails { get; set; } 
        public string Timestamp { get; set; }
        public bool Cleared { get; set; }
        public string Event { get; set; }

        public DeviceException ToDeviceException()
        {
            return new DeviceException()
            {
                Details = Details,
                AdditionalDetails = AdditionalDetails?.Split(',').ToList(),
                ErrorCode = ErrorCode,
                DeviceId = DeviceId,
                DeviceUniqueId = DeviceUniqueId,
                DeviceRepositoryId = DeviceRepositoryId,
                Timestamp = Timestamp,
                Cleared = Cleared,
                Event = Event
            };
        }
    }
}
