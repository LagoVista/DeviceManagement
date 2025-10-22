// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7f0b4b38325914e02a2248feef440e64df98b8037b6d29c8dbf8bd0bb754687d
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.Web.Common.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using System.Threading.Tasks;
using LagoVista.Core.Validation;
using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Models.Users;
using LagoVista.IoT.ProductStore;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.UserAdmin.Interfaces.Repos.Orgs;
using System.Globalization;
using Microsoft.Net.Http.Headers;
using LagoVista.IoT.DeviceManagement.Models;
using System.IO;
using LagoVista.MediaServices.Interfaces;
using LagoVista.UserAdmin.Models.Orgs;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    /// <summary>
    /// Device Group Controller
    /// </summary>
    [Authorize]
    [ConfirmedUser]
    public class DeviceRepositoryController : LagoVistaBaseController
    {
        private readonly IMediaServicesManager _mediaServicesManager;
        private readonly IDeviceRepositoryManager _deviceRepositoryManager;
        private readonly IProductStore _productStore;
        private readonly IDeviceRepositoryRepo _deviceRepoRepo;
        private readonly IOrganizationRepo _orgRepo;

        public DeviceRepositoryController(IDeviceRepositoryManager deviceReposistoryManager, IMediaServicesManager mediaServicesManager, IOrganizationRepo orgRepo, IDeviceRepositoryRepo deviceRepoRepo, IProductStore productStore, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _deviceRepositoryManager = deviceReposistoryManager ?? throw new ArgumentNullException(nameof(deviceReposistoryManager));
            _productStore = productStore ?? throw new ArgumentNullException(nameof(productStore));
            _deviceRepoRepo = deviceRepoRepo ?? throw new ArgumentNullException(nameof(deviceRepoRepo));
            _orgRepo = orgRepo ?? throw new ArgumentNullException(nameof(orgRepo));
            _mediaServicesManager = mediaServicesManager ?? throw new ArgumentNullException(nameof(mediaServicesManager));
        }

        /// <summary>
        /// Device Repositories - Add
        /// </summary>
        /// <param name="deviceGroup"></param>
        /// <returns></returns>
        [HttpPost("/api/devicerepo")]
        public Task<InvokeResult> AddDeviceRepositoryAsync([FromBody] DeviceRepository deviceGroup)
        {
            return _deviceRepositoryManager.AddDeviceRepositoryAsync(deviceGroup, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Repositories - Update
        /// </summary>
        /// <param name="deviceGroup"></param>
        /// <returns></returns>
        [HttpPut("/api/devicerepo")]
        public Task<InvokeResult> UpdateDeviceRepositoryAsync([FromBody] DeviceRepository deviceGroup)
        {
            SetUpdatedProperties(deviceGroup);
            return _deviceRepositoryManager.UpdateDeviceRepositoryAsync(deviceGroup, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Repositories - Get for Org
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/devicerepos")]
        public Task<ListResponse<DeviceRepositorySummary>> GetDeviceReposForOrgAsync()
        {
            return _deviceRepositoryManager.GetDeploymentHostsForOrgAsync(OrgEntityHeader.Id, GetListRequestFromHeader(), UserEntityHeader);
        }

        /// <summary>
        /// Device Repositories - Get for Org that are not assinged to an instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/devicerepos/available")]
        public Task<ListResponse<DeviceRepositorySummary>> GetAvailableDeviceReposForOrgAsync()
        {
            return _deviceRepositoryManager.GetAvailableDeploymentHostsForOrgAsync(OrgEntityHeader.Id, GetListRequestFromHeader(), UserEntityHeader);
        }

        /// <summary>
        /// Device Repositories - Can Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/devicerepo/{id}/inuse")]
        public Task<DependentObjectCheckResult> InUseCheck(String id)
        {
            return _deviceRepositoryManager.CheckInUseAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Repositories - Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/devicerepo/{id}")]
        public async Task<DetailResponse<DeviceRepository>> GetDeviceRepositoryAsync(String id)
        {
            var deviceRepo = await _deviceRepositoryManager.GetDeviceRepositoryAsync(id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<DeviceRepository>.Create(deviceRepo);
        }


        [HttpGet("/api/device/{devicerepoid}/connectionsettings/reset")]
        public async Task<InvokeResult<DeviceRepository>> ResetConnectionSettings(string devicerepoid)
        {
            return await _deviceRepositoryManager.ResetConnectionSettingsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// Device Repositories - Key In Use
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/devicegroups/keyinuse/{key}")]
        public Task<bool> QueryKeyInUse(String key)
        {
            return _deviceRepositoryManager.QueryKeyInUserAsync(key, OrgEntityHeader);
        }

        /// <summary>
        /// Device Repositories - Delete
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/api/devicerepo/{id}")]
        public Task<InvokeResult> DeleteDeviceGroupsAsync(string id)
        {
            return _deviceRepositoryManager.DeleteDeviceRepositoryAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        ///  Device Repositories - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/devicerepo/factory")]
        public DetailResponse<DeviceRepository> CreateDeviceRep()
        {
            var response = DetailResponse<DeviceRepository>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }

        /// <summary>
        ///  Device Repositories - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/devicerepo/standard/factory")]
        public DetailResponse<DeviceRepository> CreateStandardDeviceRep()
        {
            var response = DetailResponse<DeviceRepository>.Create();
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);
            response.Model.RepositoryType = EntityHeader<RepositoryTypes>.Create(RepositoryTypes.NuvIoT);
            return response;
        }

        [AllowAnonymous]
        [HttpGet("/api/devicerepo/{orgid}/{id}/logo/light")]
        public async Task<IActionResult> DownloadLightLogo(string orgid, string id)
        {
            var lastMod = String.Empty;
            if (!String.IsNullOrEmpty(Request.Headers["If-Modified-Since"]))
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                lastMod = DateTime.ParseExact(Request.Headers["If-Modified-Since"], "r", provider).ToJSONString();
            }

            String mediaResourceId = null;

            var repo = await _deviceRepoRepo.GetDeviceRepositoryAsync(id);
            if (!EntityHeader.IsNullOrEmpty(repo.LightLogo))
            {
                mediaResourceId = repo.LightLogo.Id;
            }
            else
            {
                var org = await _orgRepo.GetOrganizationAsync(orgid);
                if (!EntityHeader.IsNullOrEmpty(org.LightLogo))
                {
                    mediaResourceId = org.LightLogo.Id;
                }
            }

            if (!String.IsNullOrEmpty(mediaResourceId))
            {
                var response = await _mediaServicesManager.GetPublicResourceRecordAsync(orgid, mediaResourceId, lastMod);
                if (response.NotModified)
                {
                    return StatusCode(304);
                }

                var ms = new MemoryStream(response.ImageBytes);
                var idx = 1;
                foreach (var timing in response.Timings)
                {
                    Response.Headers.Add($"x-{idx++}-{timing.Key}", $"{timing.Ms}ms");
                }

                Response.Headers[HeaderNames.CacheControl] = "public";
                Response.Headers[HeaderNames.LastModified] = new[] { response.LastModified.ToDateTime().ToString("R") }; // Format RFC1123

                return File(ms, response.ContentType, response.FileName);
            }
            else
            {
                return Redirect("https://nuviot.blob.core.windows.net/cdn/nuviot-blue.png");
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/devicerepo/{orgid}/{id}/logo/dark")]
        public async Task<IActionResult> DownloadDarkLogo(string orgid, string id)
        {
            var lastMod = String.Empty;
            if (!String.IsNullOrEmpty(Request.Headers["If-Modified-Since"]))
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                lastMod = DateTime.ParseExact(Request.Headers["If-Modified-Since"], "r", provider).ToJSONString();
            }

            String mediaResourceId = null;

            var repo = await _deviceRepoRepo.GetDeviceRepositoryAsync(id);
            if (!EntityHeader.IsNullOrEmpty(repo.DarkLogo))
            {
                mediaResourceId = repo.DarkLogo.Id;
            }
            else
            {
                var org = await _orgRepo.GetOrganizationAsync(orgid);
                if (!EntityHeader.IsNullOrEmpty(org.DarkLogo))
                {
                    mediaResourceId = org.DarkLogo.Id;
                }
            }

            if (!String.IsNullOrEmpty(mediaResourceId))
            {
                var response = await _mediaServicesManager.GetPublicResourceRecordAsync(orgid, mediaResourceId, lastMod);
                if (response.NotModified)
                {
                    return StatusCode(304);
                }

                var ms = new MemoryStream(response.ImageBytes);
                var idx = 1;
                foreach (var timing in response.Timings)
                {
                    Response.Headers.Add($"x-{idx++}-{timing.Key}", $"{timing.Ms}ms");
                }

                Response.Headers[HeaderNames.CacheControl] = "public";
                Response.Headers[HeaderNames.LastModified] = new[] { response.LastModified.ToDateTime().ToString("R") }; // Format RFC1123

                return File(ms, response.ContentType, response.FileName);
            }
            else
            {
                return Redirect("https://nuviot.blob.core.windows.net/cdn/nuviot-blue.png");
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/devicerepo/{orgid}/{id}/theme")]
        public Task<InvokeResult<BasicTheme>> GetTheme(string orgid, string id)
        {
            return _deviceRepositoryManager.GetBasicThemeForRepoAsync(orgid, id);
        }
    }
}

