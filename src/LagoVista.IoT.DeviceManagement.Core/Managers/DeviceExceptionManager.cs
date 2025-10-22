// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 096628f0462d266803c7fde218ff1a12266c28ba5d663d344062da3d299ca8f9
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceExceptionManager : ManagerBase, IDeviceExceptionManager
    {
        private readonly IDeviceExceptionRepo _deviceExceptionRepo;
        private readonly IProxyFactory _proxyFactory;

        public DeviceExceptionManager(IDeviceExceptionRepo deviceExceptionRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security,
            IProxyFactory proxyFactory) : base(logger, appConfig, depmanager, security)
        {
            _deviceExceptionRepo = deviceExceptionRepo ?? throw new ArgumentNullException(nameof(deviceExceptionRepo));
            _proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
        }

        public IDeviceExceptionRepo GetDeviceExceptionRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local
                ? _proxyFactory.Create<IDeviceExceptionRepo>(
                    new ProxySettings
                    {
                        OrganizationId = deviceRepo.OwnerOrganization.Id,
                        InstanceId = deviceRepo.Instance.Id
                    }) : _deviceExceptionRepo;
        }

        public async Task<ListResponse<DeviceException>> GetDeviceExceptionsAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeviceArchive), Actions.Read);
            return await GetDeviceExceptionRepo(deviceRepo).GetDeviceExceptionsAsync(deviceRepo, deviceId, listRequest);
        }
    }
}
