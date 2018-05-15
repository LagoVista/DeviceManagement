﻿using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    /// <summary>
    /// Device Media Conroller
    /// </summary>
    [Authorize]
    public class DeviceMediaController : LagoVistaBaseController
    {
        IDeviceMediaManager _deviceMediaManager;
        IDeviceRepositoryManager _repoManager;

        public DeviceMediaController(IDeviceRepositoryManager repoManager, IDeviceMediaManager deviceMediaManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _deviceMediaManager = deviceMediaManager;
            _repoManager = repoManager;
        }

        /// <summary>
        /// Get a list of media items for adevice
        /// </summary>
        /// <param name="repoid">Repository Id</param>
        /// <param name="id">Unique Device Id</param>
        /// <returns></returns>
        [HttpGet("/api/{repoid}/devices/{id}/media")]
        public async Task<ListResponse<DeviceMedia>> GetMediaItemsForDeviceAsync(string repoid, string id)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(repoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceMediaManager.GetMediaItemsForDeviceAsync(repo, id, OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Get a list of media items for adevice
        /// </summary>
        /// <param name="repoid">Repository Id</param>
        /// <param name="id">Unique Device Id</param>
        /// <param name="mediaid">Id of Media Item</param>
        /// <returns></returns>
        [HttpGet("/api/{repoid}/devices/{id}/media/{mediaid}")]
        public async Task<IActionResult> GetMediaItemAsync(string repoid, string id, string mediaid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(repoid, OrgEntityHeader, UserEntityHeader);
            var item = await _deviceMediaManager.GetMediaItemAsync(repo, id, mediaid, OrgEntityHeader, UserEntityHeader);
            var ms = new MemoryStream(item.ImageBytes);
            return new FileStreamResult(ms, item.ContentType);
        }

        [HttpPost("/api/{repoid}/devices/{id}/media")]
        public async Task<InvokeResult> UploadMediaAsync(string repoid, string id, [FromBody] IFormFile file)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(repoid, OrgEntityHeader, UserEntityHeader);

            using (var strm = file.OpenReadStream())
            {
                return await _deviceMediaManager.AddMediaItemAsync(repo, id, strm, file.ContentType, OrgEntityHeader, UserEntityHeader);
            }
        }
    }
}
