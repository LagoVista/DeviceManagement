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

        /// <summary>Runtime service-to-service endpoint. Accepts one or many normalized readings.</summary>
        [HttpPost("/api/device/proximity/observations")]
        public async Task<InvokeResult> WriteAsync([FromBody] List<DeviceProximityObservation> observations)
        {
            if (observations == null || observations.Count == 0 || observations.Count > 100)
                return InvokeResult.FromError("A batch must contain between 1 and 100 observations.");

            if (observations.Any(item => item == null || item.TimeStamp == default(DateTime) ||
                    item.TimeStamp > DateTime.UtcNow.AddMinutes(5) ||
                    item.TimeStamp < DateTime.UtcNow.AddDays(-14) ||
                    String.IsNullOrWhiteSpace(item.OrganizationId) ||
                    String.IsNullOrWhiteSpace(item.DeviceRepoId) ||
                    String.IsNullOrWhiteSpace(item.DeviceUniqueId) ||
                    String.IsNullOrWhiteSpace(item.ObservedDeviceId)))
                return InvokeResult.FromError("Invalid proximity observation batch.");

            await _observations.WriteAsync(observations);
            return InvokeResult.Success;
        }

        /// <summary>Query a reporting device's observations using ListRequest header paging and date bounds.</summary>
        [ConfirmedUser]
        [HttpGet("/api/device/{devicerepoid}/proximity/{deviceuniqueid}")]
        public async Task<ListResponse<DeviceProximityObservation>> GetAsync(string devicerepoid, string deviceuniqueid)
        {
            // This authorization check also enforces repository ownership/visibility.
            var repo = await _deviceRepos.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _observations.QueryAsync(OrgEntityHeader.Id, repo.Id, deviceuniqueid, GetListRequestFromHeader());
        }
    }
}
