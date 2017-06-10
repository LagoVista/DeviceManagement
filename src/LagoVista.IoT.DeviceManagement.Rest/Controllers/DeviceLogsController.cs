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

        public DeviceLogsController(IDeviceLogManager deviceLogManager, UserManager<LagoVista.UserAdmin.Models.Account.AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _deviceLogManager = deviceLogManager;
        }

        /// <summary>
        /// Device Logs - Get For Device (Currently returns 100 most recent, will have filtering
        /// </summary>
        /// <param name="deviceid">Device Id</param>
        /// <returns></returns>
        [HttpGet("devicelogs/{deviceid}")]
        public async Task<ListResponse<DeviceLog>> GetDevicesForOrg(String deviceid)
        {
            //TODO: Need to add paging.
            var devices = await _deviceLogManager.GetForDateRangeAsync(deviceid);
            var response = ListResponse<DeviceLog>.Create(devices);

            return response;
        }
    }
}
