using LagoVista.CloudStorage.Storage;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceLogRepo : LagoVista.CloudStorage.Storage.TableStorageBase<DeviceLog>, IDeviceLogRepo
    {
        public DeviceLogRepo(IDeviceManagementSettings settings, IAdminLogger logger) : base(settings.DeviceManagementLogTableStorage.AccountId,  settings.DeviceManagementLogTableStorage.AccessKey, logger)
        {

        }

        public Task AddLogEntryAsync(DeviceLog logEntry)
        {
            return InsertAsync(logEntry);
        }

        public Task<IEnumerable<DeviceLog>> GetForDateRangeAsync(string deviceId, int maxReturnCount, string start, string end)
        {
            //TODO: Need to add some bounds here so it won't run forever...
            //TODO: Need to implement filtering
            //return base.GetByFilterAsync(FilterOptions.Create("DateStamp", FilterOptions.Operators.GreaterThan, start), FilterOptions.Create("DateStamp", FilterOptions.Operators.LessThan, end));

            return GetByParitionIdAsync(deviceId);
        }
    }
}
