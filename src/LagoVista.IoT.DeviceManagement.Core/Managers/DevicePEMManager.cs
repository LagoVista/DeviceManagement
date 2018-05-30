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
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DevicePEMManager : ManagerBase, IDevicePEMManager
    {
        private readonly IDevicePEMRepo _defaultDevicePEMRepo;

        private readonly IAsyncProxyFactory _asyncProxyFactory;
        private readonly IAsyncCoupler<IAsyncResponse> _asyncCoupler;
        private readonly IAsyncRequestHandler _requestSender;

        public IDevicePEMRepo GetRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local ?
                 _asyncProxyFactory.Create<IDevicePEMRepo>(
                     _asyncCoupler, 
                     _requestSender, 
                     Logger,
                     deviceRepo.Instance.Id, 
                     TimeSpan.FromSeconds(120)) :
                 _defaultDevicePEMRepo;
        }

        public DevicePEMManager(IDevicePEMRepo devicePEMRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security,
            IAsyncProxyFactory asyncProxyFactory,
            IAsyncCoupler<IAsyncResponse> asyncCoupler,
            IAsyncRequestHandler requestSender) : base(logger, appConfig, dependencyManager, security)
        {
            _defaultDevicePEMRepo = devicePEMRepo;

            _asyncProxyFactory = asyncProxyFactory;
            _asyncCoupler = asyncCoupler;
            _requestSender = requestSender;
        }

        public async Task<ListResponse<IPEMIndex>> GetPEMIndexesforDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceRepo, AuthorizeResult.AuthorizeActions.Read, user, org, "GetPEMIndexesForDevice");
            return await GetRepo(deviceRepo).GetPEMIndexForDeviceAsync(deviceRepo, deviceId, request);
        }

        public async Task<InvokeResult<string>> GetPEMAsync(DeviceRepository deviceRepo, string deviceId, string messageId, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceRepo, AuthorizeResult.AuthorizeActions.Read, user, org, "GetPEMDetail");
            //TODO: Add Security, will be authorized but we don't know if the user can get THIS pem record.

            var pemJSON = await GetRepo(deviceRepo).GetPEMAsync(deviceRepo, deviceId, messageId);
            if(pemJSON != null)
            {
                return InvokeResult<string>.Create(pemJSON);
            }
            else
            {
                return InvokeResult<string>.FromErrors(Resources.ErrorCodes.PEMDoesNotExist.ToErrorMessage($"RepoId={deviceRepo.Id},DeviceId={deviceId},MessageId={messageId}"));
            }
        }

        public async Task<ListResponse<IPEMIndex>> GetPEMIndexesforErrorReasonAsync(DeviceRepository deviceRepo, string errorReason, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceRepo, AuthorizeResult.AuthorizeActions.Read, user, org, "GetPEMIndexesForErrorReason");
            return await GetRepo(deviceRepo).GetPEMIndexForErrorReasonAsync(deviceRepo, errorReason, request);
        }
    }
}
