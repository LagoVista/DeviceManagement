using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
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

        public DeviceLogsController(IDeviceRepositoryManager repoManager, IDeviceLogManager deviceLogManager, UserManager<LagoVista.UserAdmin.Models.Account.AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _deviceLogManager = deviceLogManager;
            _repoManager = repoManager;
        }

        /// <summary>
        /// Device Logs - Get For Device (Currently returns 100 most recent, will have filtering
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="deviceid">Device Id</param>
        /// <returns></returns>
        [HttpGet("device/{devicerepoid}/logs/{deviceid}")]
        public async Task<ListResponse<DeviceLog>> GetDevicesForOrg(string devicerepoid, String deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            //TODO: Need to add paging.
            var devices = await _deviceLogManager.GetForDateRangeAsync(repo, deviceid);
            var response = ListResponse<DeviceLog>.Create(devices);

            return response;
        }
    }
}
