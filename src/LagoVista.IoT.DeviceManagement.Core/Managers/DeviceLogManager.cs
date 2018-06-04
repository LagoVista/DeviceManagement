using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Networking.AsyncMessaging;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceLogManager : ManagerBase, IDeviceLogManager
    {
        private readonly IDeviceLogRepo _defaultRepo;
        private readonly IAsyncProxyFactory _asyncProxyFactory;

        public IDeviceLogRepo GetRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local ?
                _asyncProxyFactory.Create<IDeviceLogRepo>(
                    deviceRepo.OwnerOrganization.Id,
                    deviceRepo.Instance.Id,
                    TimeSpan.FromSeconds(120)) :
                _defaultRepo;
        }

        public DeviceLogManager(IDeviceLogRepo logRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, 
            ISecurity security, IAsyncProxyFactory asyncProxyFactory) :
            base(logger, appConfig, depmanager, security)
        {
            _defaultRepo = logRepo;
            _asyncProxyFactory = asyncProxyFactory;
        }

        public Task AddEntryAsync(DeviceRepository deviceRepo, DeviceLog logEntry, EntityHeader org, EntityHeader user)
        {
            var repo = GetRepo(deviceRepo);
            return repo.AddLogEntryAsync(deviceRepo, logEntry);
        }

        public async Task<ListResponse<DeviceLog>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeviceLog), LagoVista.Core.Validation.Actions.Read);
            var repo = GetRepo(deviceRepo);
            return await repo.GetForDateRangeAsync(deviceRepo, deviceId, request);
        }
    }
}
