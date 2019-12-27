using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class FirmwareDownloadRequestRepo : TableStorageBase<FirmwareDownloadRequestDTO>
    {
        public FirmwareDownloadRequestRepo(string accountName, string accountKey, IAdminLogger logger) : 
            base(accountName, accountKey, logger)
        {

        }

        public Task AddRequestAsync(FirmwareDownloadRequest request)
        {
            return InsertAsync(FirmwareDownloadRequestDTO.FromRequest(request));
        }

        public async Task<FirmwareDownloadRequest> GetRequestAsync(string requestId)
        {
            return (await this.GetAsync(requestId)).ToDownloadRequest();
        }

        public Task UpdateRequestAsync(FirmwareDownloadRequest request)
        {
            return UpdateAsync(FirmwareDownloadRequestDTO.FromRequest(request));
        }
    }

    public class FirmwareDownloadRequestDTO : TableStorageEntity
    {
        public bool Expired { get; set; }
        public string DeviceId { get; set; }
        public string Timestamp { get; set; }
        public string ExpiresUTC { get; set; }
        public string FirmwareId { get; set; }
        public string FirmwareRevisionId { get; set; }

        public static FirmwareDownloadRequestDTO FromRequest(FirmwareDownloadRequest request)
        {
            return new FirmwareDownloadRequestDTO()
            {
                RowKey = request.DownloadId,
                PartitionKey = request.OrgId,
                Expired = request.Expired,
                DeviceId = request.DeviceId,
                Timestamp = request.Timestamp,
                ExpiresUTC = request.ExpiresUTC,
                FirmwareId = request.FirmwareId,
                FirmwareRevisionId = request.FirmwareRevisionId
            };
        }

        public FirmwareDownloadRequest ToDownloadRequest()
        {
            return new FirmwareDownloadRequest()
            {
                DownloadId = RowKey,
                OrgId = PartitionKey,
                Expired = Expired,
                DeviceId = DeviceId,
                Timestamp = Timestamp,
                ExpiresUTC = ExpiresUTC,
                FirmwareId = FirmwareId,
                FirmwareRevisionId = FirmwareRevisionId
            };
        }
    }
}
