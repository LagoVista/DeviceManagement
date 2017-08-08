using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceLogRepo : TableStorageBase<DeviceLog>, IDeviceLogRepo
    {
        public DeviceLogRepo(IAdminLogger logger) : base(logger)
        {

        }

        public Task AddLogEntryAsync(DeviceRepository deviceRepo, DeviceLog logEntry)
        {
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);

            return InsertAsync(logEntry);
        }

        public Task<ListResponse<DeviceLog>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            return GetPagedResultsAsync(deviceId, request);
        }
    }
}
