// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 338e22f1a5e87d4ea118b13dbbc5a6c6a2df34ca13bc29054fe0e706ebbed7c5
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DevicePEMManager : ManagerBase, IDevicePEMManager
    {
        private readonly IDevicePEMRepo _defaultDevicePEMRepo;

        private readonly IProxyFactory _proxyFactory;

        public IDevicePEMRepo GetRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local
                ? _proxyFactory.Create<IDevicePEMRepo>(
                    new ProxySettings
                    {
                        OrganizationId = deviceRepo.OwnerOrganization.Id,
                        InstanceId = deviceRepo.Instance.Id
                    })
                : _defaultDevicePEMRepo;
        }

        public DevicePEMManager(IDevicePEMRepo devicePEMRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security,
            IProxyFactory proxyFactory) : base(logger, appConfig, dependencyManager, security)
        {
            _defaultDevicePEMRepo = devicePEMRepo;
            _proxyFactory = proxyFactory;
        }

        public async Task<ListResponse<PEMIndex>> GetPEMIndexesforDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceRepo, AuthorizeResult.AuthorizeActions.Read, user, org, "GetPEMIndexesForDevice");
            return await GetRepo(deviceRepo).GetPEMIndexForDeviceAsync(deviceRepo, deviceId, request);
        }

        public async Task<InvokeResult<string>> GetPEMAsync(DeviceRepository deviceRepo, string deviceId, string messageId, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceRepo, AuthorizeResult.AuthorizeActions.Read, user, org, "GetPEMDetail");
            //TODO: Add Security, will be authorized but we don't know if the user can get THIS pem record.

            var pemJSON = await GetRepo(deviceRepo).GetPEMAsync(deviceRepo, deviceId, messageId);
            if (pemJSON != null)
            {
                return InvokeResult<string>.Create(pemJSON);
            }
            else
            {
                return InvokeResult<string>.FromErrors(Resources.ErrorCodes.PEMDoesNotExist.ToErrorMessage($"RepoId={deviceRepo.Id},DeviceId={deviceId},MessageId={messageId}"));
            }
        }

        public async Task<ListResponse<PEMIndex>> GetPEMIndexesforErrorReasonAsync(DeviceRepository deviceRepo, string errorReason, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceRepo, AuthorizeResult.AuthorizeActions.Read, user, org, "GetPEMIndexesForErrorReason");
            return await GetRepo(deviceRepo).GetPEMIndexForErrorReasonAsync(deviceRepo, errorReason, request);
        }
    }
}
