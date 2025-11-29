using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Threading.Tasks;
using LagoVista.IoT.DeviceManagement.Models;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    internal class SensorDataArchiveManager : ManagerBase, ISensorDataArchiveManager
    {
        private readonly ISensorDataArchiveRepo _defaultArchiveRepo;
        private readonly IProxyFactory _proxyFactory;

        public SensorDataArchiveManager(ISensorDataArchiveRepo archiveRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security,
            IProxyFactory proxyFactory) : base(logger, appConfig, depmanager, security)
        {
            _defaultArchiveRepo = archiveRepo ?? throw new ArgumentNullException(nameof(archiveRepo));
            _proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
        }

        public ISensorDataArchiveRepo GetSensorDataArchiveRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local
                ? _proxyFactory.Create<ISensorDataArchiveRepo>(
                    new ProxySettings
                    {
                        OrganizationId = deviceRepo.OwnerOrganization.Id,
                        InstanceId = deviceRepo.Instance.Id
                    })
                : _defaultArchiveRepo;
        }

        public async Task<InvokeResult> AddSensoDataArchiveAsync(DeviceRepository deviceRepo, SensorDataArchive logEntry, EntityHeader org, EntityHeader user)
        {
            await GetSensorDataArchiveRepo(deviceRepo).AddSensorDataArchiveAsync(deviceRepo, logEntry);
            return InvokeResult.Success;
        }

        public async Task<ListResponse<SensorDataArchive>> GetSensorDataArchivesAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeviceArchive), Actions.Read);
            return await GetSensorDataArchiveRepo(deviceRepo).GetForDateRangeAsync(deviceRepo, deviceId, listRequest);
        }
    }
}

