// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: db61f51eb0c25ca5b1bea5d874961f2e1c53abe935e6a83a2efe9fd0d693d5c3
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using System.Collections.Generic;
using System.Linq;
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

        public async  Task<IEnumerable<FirmwareDownloadRequest>> GetForDeviceAsync(string deviceRepoId, string deviceId)
        {
            var requests = await base.GetByFilterAsync(FilterOptions.Create(nameof(FirmwareDownloadRequestDTO.DeviceRepoId), FilterOptions.Operators.Equals, deviceRepoId),
                                  FilterOptions.Create(nameof(FirmwareDownloadRequestDTO.DeviceId), FilterOptions.Operators.Equals, deviceId));

            return requests.Select(rqst => rqst.ToDownloadRequest());
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
        public string Status { get; set; }
        public int PercentRequested { get; set; }
        public string DeviceRepoId { get; set; }
        public string DeviceId { get; set; }
        public string FirmwareName { get; set; }
        public string Timestamp { get; set; }
        public string ExpiresUTC { get; set; }
        public string FirmwareId { get; set; }
        public string FirmwareRevisionId { get; set; }
        public string Error { get; set; }

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
                FirmwareName = request.FirmwareName,
                PercentRequested = request.PercentRequested,
                Status = request.Status,
                DeviceRepoId = request.DeviceRepoId,            
                FirmwareId = request.FirmwareId,
                Error = request.Error,
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
                FirmwareName = FirmwareName,
                DeviceRepoId = DeviceRepoId,
                Status = Status,
                Error = Error,
                PercentRequested = PercentRequested,
                FirmwareRevisionId = FirmwareRevisionId
            };
        }
    }
}
