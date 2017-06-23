using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
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
    [Authorize]
    [ConfirmedUser]
    public class DevicePEMController : LagoVistaBaseController
    {
        IDevicePEMManager _devicePEMManager;

        public DevicePEMController(IDevicePEMManager devicePEMManager, UserManager<LagoVista.UserAdmin.Models.Account.AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _devicePEMManager = devicePEMManager;
        }

        /// <summary>
        /// Device PEMS - Get List of Indexs for Device Id
        /// </summary>
        /// <param name="deviceid">Device Id</param>
        /// <returns></returns>
        [HttpGet("device/pem/list/{deviceid}")]
        public async Task<ListResponse<DevicePEMIndex>> GetDevicePEMListAsync(String deviceid)
        {
            //TODO: Need to add paging.
            var pemindexes = await _devicePEMManager.GetPEMIndexesforDeviceAsync(deviceid, OrgEntityHeader, UserEntityHeader);
            var response = ListResponse<DevicePEMIndex>.Create(pemindexes);

            return response;
        }


        /// <summary>
        /// Device PEMS - Get PEM for URL (passed in body)
        /// </summary>
        /// <param name="pemuri">Device Id</param>
        /// <returns></returns>
        [HttpGet("device/pem")]
        public  Task<InvokeResult<string>> GetDevicePEMAsync([FromBody] String pemuri)
        {
            return _devicePEMManager.GetPEMAsync(pemuri,  OrgEntityHeader, UserEntityHeader);
        }

    }
}
