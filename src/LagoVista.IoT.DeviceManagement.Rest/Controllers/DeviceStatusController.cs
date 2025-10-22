// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 77452fd124c534590cbe04e57431251942f087ccddb8dd328aa98735858885a9
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    [Authorize]
    [ConfirmedUser]
    public class DeviceStatusController : LagoVistaBaseController
    {
        IDeviceStatusManager _deviceStatusManager;
        IDeviceRepositoryManager _repoManager;

        public DeviceStatusController(IDeviceStatusManager statusManager, IDeviceRepositoryManager repoManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _deviceStatusManager = statusManager ?? throw new ArgumentNullException(nameof(statusManager));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
        }

        [HttpGet("/api/device/{devicerepoid}/status/{deviceid}/history")]
        public async Task<ListResponse<DeviceStatus>> GetDeviceStatusUpdatesAsync(string devicerepoid, String deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceStatusManager.GetDeviceStatusHistoryAsync(repo, deviceid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }


        [HttpGet("/api/devices/{devicerepoid}/status")]
        public async Task<ListResponse<DeviceStatus>> GetDeviceWatchDogStatusAsync(string devicerepoid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceStatusManager.GetWatchdogDeviceStatusAsync(repo, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        [HttpPut("/api/device/{devicerepoid}/status")]
        public async Task<InvokeResult> UpdateDeviceStatusAsync(string devicerepoid, [FromBody] DeviceStatus deviceStatus)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceStatusManager.UpdateDeviceStatusAsync(repo, deviceStatus, OrgEntityHeader, UserEntityHeader);
        }

        [HttpGet("/api/device/{devicerepoid}/watchdog/{deviceid}/notifications/{silenced}")]
        public async Task<InvokeResult> EnableDisableDeviceWatchdogNotificationsAsync(string devicerepoid, string deviceid, bool silenced)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceStatusManager.SetSilenceAlarmAsync(repo, deviceid, silenced, OrgEntityHeader, UserEntityHeader);
        }
    }
}
