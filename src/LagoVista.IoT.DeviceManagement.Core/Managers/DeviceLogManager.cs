using System.Collections.Generic;
using LagoVista.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Interfaces;
using System.Threading.Tasks;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;

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

        public Task AddEntryAsync(DeviceRepository deviceRepo, DeviceLog logEntry)
        {
            return _logRepo.AddLogEntryAsync(deviceRepo, logEntry);
        }

        public Task<IEnumerable<DeviceLog>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, int maxReturnCount = 100, string start = null, string end = null)
        {
            return _logRepo.GetForDateRangeAsync(deviceRepo, deviceId, maxReturnCount, start, end);
        }
    }
}
