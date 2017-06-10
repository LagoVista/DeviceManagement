using System;
using System.Collections.Generic;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.Managers;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Interfaces;
using System.Threading.Tasks;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceArchiveManager : ManagerBase, IDeviceArchiveManager
    {
        IDeviceArchiveRepo _archiveRepo;

        public DeviceArchiveManager(IDeviceArchiveRepo archiveRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _archiveRepo = archiveRepo;
        }

        public Task AddArchiveAsync(DeviceArchive logEntry)
        {
            return _archiveRepo.AddArchiveAsync(logEntry);
        }       

        public Task<IEnumerable<DeviceArchive>> GetForDateRangeAsync(string deviceId, int maxReturnCount = 100, string start = null, string end = null)
        {
            return _archiveRepo.GetForDateRangeAsync(deviceId, maxReturnCount, start, end);
        }
    }
}
