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
        private readonly IDeviceExceptionRepo _deviceExceptionRepo;
        private readonly IDeviceStatusChangeRepo _deviceStatusChangeRepo;
        private readonly IProxyFactory _proxyFactory;

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

        public IDeviceStatusChangeRepo GetDeviceStatusChangeRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local
                ? _proxyFactory.Create<IDeviceStatusChangeRepo>(
                    new ProxySettings
                    {
                        OrganizationId = deviceRepo.OwnerOrganization.Id,
                        InstanceId = deviceRepo.Instance.Id
                    })
                : _deviceStatusChangeRepo;
        }

        public IDeviceExceptionRepo GetDeviceExceptionRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local
                ? _proxyFactory.Create<IDeviceExceptionRepo>(
                    new ProxySettings
                    {
                        OrganizationId = deviceRepo.OwnerOrganization.Id,
                        InstanceId = deviceRepo.Instance.Id
                    })
                : _deviceExceptionRepo;
        }

        public DeviceArchiveManager(IDeviceArchiveRepo archiveRepo, IDeviceExceptionRepo deviceExceptionRepo, IDeviceStatusChangeRepo statusChangeRepo,
            IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security,
            IProxyFactory proxyFactory) : base(logger, appConfig, depmanager, security)
        {
            _defaultArchiveRepo = archiveRepo ?? throw new ArgumentNullException(nameof(archiveRepo));
            _proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            _deviceExceptionRepo = deviceExceptionRepo ?? throw new ArgumentNullException(nameof(deviceExceptionRepo));
            _deviceStatusChangeRepo = statusChangeRepo ?? throw new ArgumentNullException(nameof(statusChangeRepo));
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

        public async Task<ListResponse<DeviceException>> GetDeviceExceptionAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeviceArchive), LagoVista.Core.Validation.Actions.Read);
            return await GetDeviceExceptionRepo(deviceRepo).GetDeviceExceptionsAsync(deviceRepo, deviceId, listRequest);
        }

        public async Task<ListResponse<DeviceStatus>> GetDeviceStatusChangesAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeviceArchive), LagoVista.Core.Validation.Actions.Read);
            return await GetDeviceStatusChangeRepo(deviceRepo).GetDeviceStatusHistoryAsync(deviceRepo, deviceId, listRequest);
        }
    }
}
