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
        public DeviceLogRepo(IAdminLogger logger) : base(logger)
        {

        }

        public Task AddLogEntryAsync(DeviceRepository deviceRepo, DeviceLog logEntry)
        {
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);

            return InsertAsync(logEntry);
        }

        public Task<IEnumerable<DeviceLog>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, int maxReturnCount, string start, string end)
        {
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);

            //TODO: Need to add some bounds here so it won't run forever...
            //TODO: Need to implement filtering
            //return base.GetByFilterAsync(FilterOptions.Create("DateStamp", FilterOptions.Operators.GreaterThan, start), FilterOptions.Create("DateStamp", FilterOptions.Operators.LessThan, end));

            return GetByParitionIdAsync(deviceId);
        }
    }
}
