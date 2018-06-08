using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceArchiveManager : ManagerBase, IDeviceArchiveManager
    {
        private readonly IDeviceArchiveRepo _defaultArchiveRepo;
        private readonly IProxyFactory _proxyFactory;

        public IDeviceArchiveRepo GetDeviceArchivepRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value ==
                RepositoryTypes.Local ?
                    _proxyFactory.Create<IDeviceArchiveRepo>(new ProxySettings
                    {
                        OrganizationId = deviceRepo.OwnerOrganization.Id,
                        InstanceId = deviceRepo.Instance.Id
                    }) :
                    _defaultArchiveRepo;
        }

        public DeviceArchiveManager(IDeviceArchiveRepo archiveRepo,
            IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security,
            IProxyFactory proxyFactory) : base(logger, appConfig, depmanager, security)
        {
            _defaultArchiveRepo = archiveRepo;
            _proxyFactory = proxyFactory;
        }

        public async Task<InvokeResult> AddArchiveAsync(DeviceRepository deviceRepo, DeviceArchive logEntry, EntityHeader org, EntityHeader user)
        {
            await GetDeviceArchivepRepo(deviceRepo).AddArchiveAsync(deviceRepo, logEntry);
            return InvokeResult.Success;
        }

        public async Task<ListResponse<List<object>>> GetDeviceArchivesAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeviceArchive), LagoVista.Core.Validation.Actions.Read);
            return await GetDeviceArchivepRepo(deviceRepo).GetForDateRangeAsync(deviceRepo, deviceId, listRequest);
        }
    }
}
