using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.Managers;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DevicePEMManager : ManagerBase, IDevicePEMManager
    {
        IDevicePEMRepo _devicePEMRepo;

        public DevicePEMManager(IDevicePEMRepo devicePEMRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) : base(logger, appConfig, dependencyManager, security)
        {
            _devicePEMRepo = devicePEMRepo;
        }

        public async Task<ListResponse<IPEMIndex>> GetPEMIndexesforDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceRepo, AuthorizeResult.AuthorizeActions.Read, user, org, "GetPEMIndexesForDevice");
            return await _devicePEMRepo.GetPEMIndexForDeviceAsync(deviceRepo, deviceId, request);
        }

        public async Task<InvokeResult<string>> GetPEMAsync(DeviceRepository deviceRepo, string deviceId, string messageId, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceRepo, AuthorizeResult.AuthorizeActions.Read, user, org, "GetPEMDetail");
            //TODO: Add Security, will be authorized but we don't know if the user can get THIS pem record.

            var pemJSON = await _devicePEMRepo.GetPEMAsync(deviceRepo, deviceId, messageId);
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
            return await _devicePEMRepo.GetPEMIndexForErrorReasonAsync(deviceRepo, errorReason, request);
        }
    }
}
