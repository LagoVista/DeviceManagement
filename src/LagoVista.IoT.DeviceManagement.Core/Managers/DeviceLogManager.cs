using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Rpc.Client;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceLogManager : ManagerBase, IDeviceLogManager
    {
        private readonly IDeviceLogRepo _defaultRepo;
        private readonly IProxyFactory _proxyFactory;

        public IDeviceLogRepo GetRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local ?
                _proxyFactory.Create<IDeviceLogRepo>(
                    new ProxySettings
                    {
                        OrganizationId = deviceRepo.OwnerOrganization.Id,
                        InstanceId = deviceRepo.Instance.Id
                    }) :
                _defaultRepo;
        }

        public DeviceLogManager(IDeviceLogRepo logRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager,
            ISecurity security, IProxyFactory proxyFactory) :
            base(logger, appConfig, depmanager, security)
        {
            _defaultRepo = logRepo;
            _proxyFactory = proxyFactory;
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
