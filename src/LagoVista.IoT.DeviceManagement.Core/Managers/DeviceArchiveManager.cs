using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Networking.AsyncMessaging;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceArchiveManager : ManagerBase, IDeviceArchiveManager
    {
        private readonly IDeviceArchiveRepo _defaultArchiveRepo;
        private readonly IAsyncProxyFactory _asyncProxyFactory;
        private readonly IAsyncCoupler<IAsyncResponse> _asyncCoupler;
        private readonly IAsyncRequestHandler _requestSender;

        public IDeviceArchiveRepo GetDeviceArchivepRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value ==
                RepositoryTypes.Local ?
                    _asyncProxyFactory.Create<IDeviceArchiveRepo>(
                        _asyncCoupler,
                        _requestSender,
                        Logger,
                         $"{{\"organizationId\": \"{deviceRepo.OwnerOrganization.Id}\", \"instanceId\": \"{deviceRepo.Instance.Id}\"}}",
                        TimeSpan.FromSeconds(120)) :
                    _defaultArchiveRepo;
        }

        public DeviceArchiveManager(IDeviceArchiveRepo archiveRepo,
            IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security,
            IAsyncProxyFactory asyncProxyFactory,
            IAsyncCoupler<IAsyncResponse> asyncCoupler,
            IAsyncRequestHandler requestSender) : base(logger, appConfig, depmanager, security)
        {
            _defaultArchiveRepo = archiveRepo;

            _asyncProxyFactory = asyncProxyFactory;
            _asyncCoupler = asyncCoupler;
            _requestSender = requestSender;
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
