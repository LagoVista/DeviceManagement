using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    /// <summary>
    /// Device Logs Services
    /// </summary>
    [Authorize]
    [ConfirmedUser]
    public class DeviceArchivesController : LagoVistaBaseController
    {
        IDeviceArchiveManager _deviceArchiveManager;
        IDeviceRepositoryManager _repoManager;

        public DeviceArchivesController(IDeviceRepositoryManager repoManager, IDeviceArchiveManager deviceArchiveManager, UserManager<LagoVista.UserAdmin.Models.Account.AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _deviceArchiveManager = deviceArchiveManager;
            _repoManager = repoManager;
        }

        /// <summary>
        /// Device Archives - Get For Device (Currently returns 100 most recent, will have filtering
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="deviceid">Device Id</param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/archives/{deviceid}")]
        public async Task<ListResponse<DeviceArchive>> GetDevicesForOrg(string devicerepoid, String deviceid)
        {
            //TODO: Need to add paging.
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var deviceArchives = await _deviceArchiveManager.GetForDateRangeAsync(repo, deviceid);
            var response = ListResponse<DeviceArchive>.Create(deviceArchives);

            return response;
        }
    }

}
