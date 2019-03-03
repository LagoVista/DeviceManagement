using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    /// <summary>
    /// Device Media Conroller
    /// </summary>
    [Authorize]
    public class AppsDeviceMediaController : ClientAppAPIController
    {
        IDeviceMediaManager _deviceMediaManager;
      
        public AppsDeviceMediaController(IDeviceRepositoryManager repoManager, IDeviceMediaManager deviceMediaManager,
            UserManager<AppUser> userManager, IAdminLogger logger) 
            : base(repoManager, userManager, logger)
        {
            _deviceMediaManager = deviceMediaManager;
        }

        /// <summary>
        /// Get a list of media items for adevice
        /// </summary>
        /// <param name="id">Unique Device Id</param>
        /// <returns></returns>
        [HttpGet("/clientapi/devices/{id}/media")]
        public async Task<ListResponse<DeviceMedia>> GetMediaItemsForDeviceAsync(string id)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceMediaManager.GetMediaItemsForDeviceAsync(repo, id, OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Get a list of media items for adevice
        /// </summary>
        /// <param name="id">Unique Device Id</param>
        /// <param name="mediaid">Id of Media Item</param>
        /// <returns></returns>
        [HttpGet("/clientapi/devices/{id}/media/{mediaid}")]
        public async Task<IActionResult> GetMediaItemAsync(string id, string mediaid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            var item = await _deviceMediaManager.GetMediaItemAsync(repo, id, mediaid, OrgEntityHeader, UserEntityHeader);
            var ms = new MemoryStream(item.ImageBytes);
            return new FileStreamResult(ms, item.ContentType);
        }
    }
}
