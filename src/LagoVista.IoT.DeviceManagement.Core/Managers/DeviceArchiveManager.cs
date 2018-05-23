using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Networking.AsyncMessaging;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
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

        private readonly IAsyncProxyFactory _remoteProxyFactory;
        private readonly IAsyncCoupler<IAsyncResponse> _asyncCoupler;
        private readonly IAsyncRequestHandler _requestHandler;

        public IDeviceArchiveRepo GetDeviceArchivepRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local ?
                 _remoteProxyFactory.Create<IDeviceArchiveRepo>(_asyncCoupler, _requestHandler) :
                 _defaultArchiveRepo;
        }

        public DeviceArchiveManager(IDeviceArchiveRepo archiveRepo, 
            IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security,
            IAsyncProxyFactory remoteProxyFactory,
            IAsyncCoupler<IAsyncResponse> asyncCoupler,
            IAsyncRequestHandler responseHandler) : base(logger, appConfig, depmanager, security)
        {
            _defaultArchiveRepo = archiveRepo;

            _remoteProxyFactory = remoteProxyFactory;
            _asyncCoupler = asyncCoupler;
            _requestHandler = responseHandler;
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
