using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    /// <summary>
    /// ClientAPI Device Logs Services
    /// </summary>
    [Authorize(AuthenticationSchemes = "APIToken")]
    [ConfirmedUser]
    public class AppsDeviceArchivesController : ClientAppAPIController
    {
        IDeviceArchiveManager _deviceArchiveManager;

        public AppsDeviceArchivesController(IDeviceRepositoryManager repoManager, 
            IDeviceArchiveManager deviceArchiveManager, UserManager<AppUser> userManager, IAdminLogger logger) 
            : base(repoManager, userManager, logger)
        {
            _deviceArchiveManager = deviceArchiveManager;
        }

        /// <summary>
        /// Client API Device Archives - Get For Device (Currently returns 100 most recent, will have filtering
        /// </summary>
        /// <param name="deviceid">Device Id</param>
        /// <returns></returns>
        [HttpGet("/clientapi/device/archives/{deviceid}")]
        public async Task<ListResponse<List<object>>> GetDeviceDataAsync(String deviceid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceArchiveManager.GetDeviceArchivesAsync(repo, deviceid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }
    }

}
