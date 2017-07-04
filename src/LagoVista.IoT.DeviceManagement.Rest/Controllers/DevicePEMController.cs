using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    [Authorize]
    [ConfirmedUser]
    public class DevicePEMController : LagoVistaBaseController
    {
        IDevicePEMManager _devicePEMManager;
        IDeviceRepositoryManager _repoManager;

        public DevicePEMController(IDeviceRepositoryManager repoManager, IDevicePEMManager devicePEMManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _repoManager = repoManager;
            _devicePEMManager = devicePEMManager;
        }

        /// <summary>
        /// Device PEMS - Get List of Indexs for Device Id
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="deviceid">Device Id</param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/pems/{deviceid}")]
        public async Task<ListResponse<DevicePEMIndex>> GetDevicePEMListAsync(string devicerepoid, String deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);

            //TODO: Need to add paging.
            var pemindexes = await _devicePEMManager.GetPEMIndexesforDeviceAsync(repo, deviceid, OrgEntityHeader, UserEntityHeader);
            var response = ListResponse<DevicePEMIndex>.Create(pemindexes);

            return response;
        }


        /// <summary>
        /// Device PEMS - Get PEM for URL (passed in body)
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="pemuri">Device Id</param>
        /// <returns></returns>
        [HttpPost("/api/device/{devicerepoid}/pem")]
        public async Task<InvokeResult<string>> GetDevicePEMAsync(String devicerepoid, [FromBody] DevicePEMRequest pemuri)
        {
            Console.WriteLine("=====> Looking for URI => " + pemuri);
            Console.WriteLine("=====> IN REPO ID      => " + devicerepoid);

            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _devicePEMManager.GetPEMAsync(repo, pemuri.PEM_URI,  OrgEntityHeader, UserEntityHeader);
        }

    }
}
