// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 47ee078de16d863bf90e11a5ebf0e8556b73b04cad798fb3740d759bd6ae4124
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core;
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceArchiveManager : ManagerBase, IDeviceArchiveManager
    {
        private readonly IDeviceArchiveRepo _defaultArchiveRepo;
        private readonly IProxyFactory _proxyFactory;

        public DeviceArchiveManager(IDeviceArchiveRepo archiveRepo, IDeviceExceptionRepo deviceExceptionRepo, IDeviceStatusChangeRepo statusChangeRepo,
            IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security,
            IProxyFactory proxyFactory) : base(logger, appConfig, depmanager, security)
        {
            _defaultArchiveRepo = archiveRepo ?? throw new ArgumentNullException(nameof(archiveRepo));
            _proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
        }

        public IDeviceArchiveRepo GetDeviceArchivepRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local
                ? _proxyFactory.Create<IDeviceArchiveRepo>(
                    new ProxySettings
                    {
                        OrganizationId = deviceRepo.OwnerOrganization.Id,
                        InstanceId = deviceRepo.Instance.Id
                    })
                : _defaultArchiveRepo;
        }

        public async Task<InvokeResult> AddArchiveAsync(DeviceRepository deviceRepo, DeviceArchive logEntry, EntityHeader org, EntityHeader user)
        {
            await GetDeviceArchivepRepo(deviceRepo).AddArchiveAsync(deviceRepo, logEntry);
            return InvokeResult.Success;
        }

        public async Task<ListResponse<List<object>>> GetDeviceArchivesAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeviceArchive), Actions.Read);
            return await GetDeviceArchivepRepo(deviceRepo).GetForDateRangeAsync(deviceRepo, deviceId, listRequest);
        }
    }
}
