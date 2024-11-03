using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using LagoVista.Core.Validation;
using System.Linq;
using LagoVista.Core;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    [Authorize]
    [ConfirmedUser]
    public class DeviceExceptionController : LagoVistaBaseController
    {
        IDeviceExceptionManager _exceptionManager;
        IDeviceRepositoryManager _repoManager;
        LagoVista.IoT.DeviceManagement.Core.IDeviceManager _deviceManager;

        public DeviceExceptionController(IDeviceExceptionManager exceptionManager, LagoVista.IoT.DeviceManagement.Core.IDeviceManager deviceManager, IDeviceRepositoryManager repoManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _exceptionManager = exceptionManager ?? throw new ArgumentNullException(nameof(exceptionManager));
        }

        [HttpGet("/api/device/{devicerepoid}/errors/{deviceid}")]
        public async Task<ListResponse<DeviceException>> GetDeviceErrorsAsync(string devicerepoid, String deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _exceptionManager.GetDeviceExceptionsAsync(repo, deviceid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        [HttpGet("/api/device/{devicerepoid}/errors/{deviceid}/silence/{errorid}")]
        public async Task<InvokeResult> SilenceErrorAsync(string devicerepoid, String deviceid, string errorid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.SilenceErrorAsync(repo, deviceid, errorid, OrgEntityHeader, UserEntityHeader);
        }
    }
}
