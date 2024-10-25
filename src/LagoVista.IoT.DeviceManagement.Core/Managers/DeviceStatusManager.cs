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
    public class DeviceStatusManager : ManagerBase, IDeviceStatusManager
    {
        private readonly IDeviceStatusChangeRepo _deviceStatusChangeRepo;
        private readonly IProxyFactory _proxyFactory;

        public DeviceStatusManager(IDeviceArchiveRepo archiveRepo, IDeviceStatusChangeRepo deviceStatusChangeRepo, IDeviceStatusChangeRepo statusChangeRepo,
            IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security,
            IProxyFactory proxyFactory) : base(logger, appConfig, depmanager, security)
        {
            _deviceStatusChangeRepo = deviceStatusChangeRepo ?? throw new ArgumentNullException(nameof(deviceStatusChangeRepo));
            _proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
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

        public async Task<ListResponse<DeviceStatus>> GetDeviceStatusHistoryAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeviceArchive), Actions.Read);
            return await GetDeviceStatusChangeRepo(deviceRepo).GetDeviceStatusHistoryAsync(deviceRepo, deviceId, listRequest);
        }

        public async Task<ListResponse<DeviceStatus>> GetWatchdogDeviceStatusAsync(DeviceRepository deviceRepo, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeviceArchive), Actions.Read);
            return await GetDeviceStatusChangeRepo(deviceRepo).GetWatchdogDeviceStatusAsync(deviceRepo, listRequest);
        }

        public async Task<InvokeResult> SetSilenceAlarmAsync(DeviceRepository deviceRepo, string id, bool silenced, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeviceArchive), Actions.Read);
            var deviceStatus = await GetDeviceStatusChangeRepo(deviceRepo).GetDeviceStatusAsync(deviceRepo, id);
            deviceStatus.SilenceAlarm = silenced;
            await GetDeviceStatusChangeRepo(deviceRepo).UpdateDeviceStatusAsync(deviceRepo, deviceStatus);
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateDeviceStatusAsync(DeviceRepository deviceRepo, DeviceStatus deviceStatus, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeviceArchive), Actions.Read);
            await GetDeviceStatusChangeRepo(deviceRepo).UpdateDeviceStatusAsync(deviceRepo, deviceStatus);
            return InvokeResult.Success;
        }
    }
}
