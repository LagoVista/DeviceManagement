// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8110e3f3e19e83eea7d3514507a5cf5f825111e62c7ec1e7d56bf234cff046a4
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.DeviceManagement.Repos.DTOs;
using LagoVista.IoT.Logging.Loggers;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceStatusChangeRepo : TableStorageBase<DeviceStatusDTO>, IDeviceStatusChangeRepo
    {
        public DeviceStatusChangeRepo(IAdminLogger logger) : base(logger)
        {
        }

        public Task AddDeviceStatusAsync(DeviceRepository deviceRepo, DeviceStatus status)
        {
            SetTableName(deviceRepo.GetDeviceCurrentStatusStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);

            return InsertAsync(new DeviceStatusDTO(status, status.DeviceUniqueId));
        }

        public Task UpdateDeviceStatusAsync(DeviceRepository deviceRepo, DeviceStatus status)
        {
            SetTableName(deviceRepo.GetDeviceCurrentStatusStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);

            return UpdateAsync(new DeviceStatusDTO(status, status.DeviceUniqueId));
        }

        public async Task<ListResponse<DeviceStatus>> GetDeviceStatusHistoryAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            SetTableName(deviceRepo.GetDeviceStatusHistoryStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);

            var result = await base.GetPagedResultsAsync(deviceId, request);
            return new ListResponse<DeviceStatus>()
            {
                Model = result.Model.Select(dto => dto.ToDeviceStatus()),
                NextPartitionKey = result.NextPartitionKey,
                NextRowKey = result.NextRowKey,
                PageIndex = result.PageIndex,
                PageCount = result.PageCount,
                PageSize = result.PageSize
            };
        }

        public async Task<DeviceStatus> GetDeviceStatusAsync(DeviceRepository deviceRepo, string deviceUniqueId)
        {
            SetTableName(deviceRepo.GetDeviceCurrentStatusStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);

            var dto = await GetAsync(deviceUniqueId);
            return dto.ToDeviceStatus();
        }

        public async Task<ListResponse<DeviceStatus>> GetWatchdogDeviceStatusAsync(DeviceRepository deviceRepo, ListRequest request)
        {
            SetTableName(deviceRepo.GetDeviceCurrentStatusStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            var result = await base.GetByFilterAsync();
            return new ListResponse<DeviceStatus>()
            {
                Model = result.Select(dto => dto.ToDeviceStatus()),
            };
        }
    }
}
