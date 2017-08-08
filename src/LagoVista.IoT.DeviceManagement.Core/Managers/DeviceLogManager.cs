using System.Collections.Generic;
using LagoVista.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.Core.Interfaces;
using System.Threading.Tasks;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceLogManager : ManagerBase,  IDeviceLogManager
    {
        IDeviceLogRepo _logRepo;

        public DeviceLogManager(IDeviceLogRepo logRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) :
            base(logger, appConfig, depmanager, security)
        {
            _logRepo = logRepo;
        }

        public Task AddEntryAsync(DeviceRepository deviceRepo, DeviceLog logEntry, EntityHeader org, EntityHeader user)
        {
            return _logRepo.AddLogEntryAsync(deviceRepo, logEntry);
        }

        public async Task<ListResponse<DeviceLog>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeviceLog), LagoVista.Core.Validation.Actions.Read);
            return await _logRepo.GetForDateRangeAsync(deviceRepo, deviceId, request);
        }
    }
}
