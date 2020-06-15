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
    public class DeviceStatusChangeRepo : LagoVista.CloudStorage.Storage.TableStorageBase<DeviceStatusDTO>, IDeviceStatusChangeRepo
    {
        public DeviceStatusChangeRepo(IAdminLogger logger) : base(logger)
        {
        }

        public Task AddStatusChanged(DeviceRepository deviceRepo, DeviceStatus status)
        {
            SetTableName(deviceRepo.GetDeviceStatusStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            return InsertAsync(new DeviceStatusDTO(status));

        }

        public async Task<ListResponse<DeviceStatus>> GetDeviceStatusHistoryAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            SetTableName(deviceRepo.GetDeviceExceptionsStorageName());
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
    }
}
