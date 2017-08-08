using LagoVista.IoT.DeviceManagement.Core.Repos;
using System.Collections.Generic;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.PlatformSupport;
using System.Threading.Tasks;
using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceArchiveRepo : LagoVista.CloudStorage.Storage.TableStorageBase<DeviceArchive>, IDeviceArchiveRepo
    {
        public DeviceArchiveRepo(IAdminLogger logger) : base(logger)
        {

        }

        public Task AddArchiveAsync(DeviceRepository deviceRepo, DeviceArchive archiveEntry)
        {
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            return InsertAsync(archiveEntry);
        }

        public Task<ListResponse<DeviceArchive>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            //TODO: Need to implement filtering
            //TODO: Need to add some bounds here so it won't run forever.
            //return base.GetByFilterAsync(FilterOptions.Create("DateStamp", FilterOptions.Operators.GreaterThan, start), FilterOptions.Create("DateStamp", FilterOptions.Operators.LessThan, end));
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            var json = GetRawJSONByParitionIdAsync(deviceId);
            return null;
        }
    }
}
