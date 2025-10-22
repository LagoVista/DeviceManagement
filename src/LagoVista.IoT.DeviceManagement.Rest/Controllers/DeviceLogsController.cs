// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f9f9db9ae1e3940615765b70e49b359d451c402b8c37acacee3287cabc7e89fa
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
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
    /// <summary>
    /// Device Logs Services
    /// </summary>
    [Authorize]
    [ConfirmedUser]
    public class DeviceLogsController : LagoVistaBaseController
    {
        IDeviceLogManager _deviceLogManager;
        IDeviceRepositoryManager _repoManager;

        public DeviceLogsController(IDeviceRepositoryManager repoManager, IDeviceLogManager deviceLogManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _deviceLogManager = deviceLogManager;
            _repoManager = repoManager;
        }

        /// <summary>
        /// Device Logs - Get For Device
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="deviceid">Device Id</param>
        /// <returns></returns>
        [HttpGet("device/{devicerepoid}/logs/{deviceid}")]
        public async Task<ListResponse<DeviceLog>> GetDevicesForOrg(string devicerepoid, String deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceLogManager.GetForDateRangeAsync(repo, deviceid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }
    }
}
