using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    [Authorize]
    [ConfirmedUser]
    public class DevicePEMController : LagoVistaBaseController
    {
        IDevicePEMManager _devicePEMManager;
        IDeviceRepositoryManager _repoManager;

        public DevicePEMController(IDeviceRepositoryManager repoManager, IDevicePEMManager devicePEMManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _repoManager = repoManager;
            _devicePEMManager = devicePEMManager;
        }

        /// <summary>
        /// Device PEMS - Get List of Indexs for Device Id
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="deviceid">Device Id</param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/pems/{deviceid}")]
        public async Task<ListResponse<IPEMIndex>> GetDevicePEMListAsync(string devicerepoid, string deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _devicePEMManager.GetPEMIndexesforDeviceAsync(repo, deviceid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device PEMS - Get PEMS By Error Reason (None, UnexepctedError, Unspecified, SeeErrorLog, CouldNotDetermineDeviceId, CouldNotDetermineMessageId, CouldNotFindDevice, CouldNotFindMessage)
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="errorreason">Error Reason</param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/pems/errors/{errorreason}")]
        public async Task<ListResponse<IPEMIndex>> GetErrorsForRepoAsync(string devicerepoid, string errorreason)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _devicePEMManager.GetPEMIndexesforErrorReasonAsync(repo, errorreason, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// Device PEMS - will need to replace period (.) with a (_) in the REST path before sending
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="deviceid"></param>
        /// <param name="pemid"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/{deviceid}/{pemid}/pem")]
        public async Task<InvokeResult<string>> GetDevicePEMAsync(String devicerepoid, string deviceid, string pemid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _devicePEMManager.GetPEMAsync(repo, deviceid, pemid.Replace("_", "."), OrgEntityHeader, UserEntityHeader);
        }        
    }
}
