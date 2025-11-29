// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c97ee019f07ec1f8f80d980e310fa2b6601924e95a782d0a71ca90417a5da3a8
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
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
    /// Device Logs Services
    /// </summary>
    [Authorize]
    [ConfirmedUser]
    public class SensorDataArchivesController : LagoVistaBaseController
    {
        ISensorDataArchiveManager _archiveManager;
        IDeviceRepositoryManager _repoManager;

        public SensorDataArchivesController(IDeviceRepositoryManager repoManager, ISensorDataArchiveManager archiveManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _archiveManager = archiveManager ?? throw new ArgumentNullException(nameof(archiveManager));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
        }

        /// <summary>
        /// Device Archives - Get For Device (Currently returns 100 most recent, will have filtering
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="deviceid">Device Id</param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/archives/sensor/{deviceid}")]
        public async Task<ListResponse<SensorDataArchive>> GetDeviceDataAsync(string devicerepoid, String deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _archiveManager.GetSensorDataArchivesAsync(repo, deviceid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }
    }
}
