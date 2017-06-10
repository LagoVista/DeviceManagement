using LagoVista.IoT.DeviceManagement.Core.Repos;
using System.Collections.Generic;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.PlatformSupport;
using System.Threading.Tasks;
using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceArchiveRepo : LagoVista.CloudStorage.Storage.TableStorageBase<DeviceArchive>, IDeviceArchiveRepo
    {
        public DeviceArchiveRepo(IDeviceManagementSettings settings, IAdminLogger logger) : base(settings.DeviceManagementArchiveTableStorage.AccountId,  settings.DeviceManagementArchiveTableStorage.AccessKey, logger)
        {

        }

        public Task AddArchiveAsync(DeviceArchive archiveEntry)
        {
            return InsertAsync(archiveEntry);
        }

        public Task<IEnumerable<DeviceArchive>> GetForDateRangeAsync(string deviceId, int maxReturnCount, string start, string end)
        {
            //TODO: Need to implement filtering
            //TODO: Need to add some bounds here so it won't run forever.
            //return base.GetByFilterAsync(FilterOptions.Create("DateStamp", FilterOptions.Operators.GreaterThan, start), FilterOptions.Create("DateStamp", FilterOptions.Operators.LessThan, end));

            return GetByParitionIdAsync(deviceId);
        }
    }
}
