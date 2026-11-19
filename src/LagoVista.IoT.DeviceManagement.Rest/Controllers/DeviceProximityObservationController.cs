using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    [Authorize]
    public class DeviceProximityObservationController : LagoVistaBaseController
    {
        private readonly IDeviceProximityObservationRepo _observations;
        private readonly IDeviceRepositoryManager _deviceRepos;

        public DeviceProximityObservationController(IDeviceProximityObservationRepo observations,
            IDeviceRepositoryManager deviceRepos, UserManager<AppUser> userManager, IAdminLogger logger)
            : base(userManager, logger)
        {
            _observations = observations ?? throw new ArgumentNullException(nameof(observations));
            _deviceRepos = deviceRepos ?? throw new ArgumentNullException(nameof(deviceRepos));
        }

        /// <summary>Query a reporting device's observations using ListRequest header paging and date bounds.</summary>
        [ConfirmedUser]
        [HttpGet("/api/device/{devicerepoid}/proximity/{deviceuniqueid}")]
        public async Task<ListResponse<DeviceProximityObservation>> GetAsync(string devicerepoid, string deviceuniqueid,
            [FromQuery] DateTimeOffset? startUtc = null, [FromQuery] DateTimeOffset? endUtc = null)
        {
            // This authorization check also enforces repository ownership/visibility.
            var repo = await _deviceRepos.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _observations.QueryAsync(OrgEntityHeader.Id, repo.Id, deviceuniqueid,
                GetListRequestFromHeader(), startUtc?.UtcDateTime, endUtc?.UtcDateTime);
        }
    }
}
