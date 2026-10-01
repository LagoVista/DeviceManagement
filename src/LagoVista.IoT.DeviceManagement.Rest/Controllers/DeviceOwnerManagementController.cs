using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core;
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
    public class DeviceOwnerManagementController : LagoVistaBaseController
    {
        private readonly IDeviceOwnerManager _deviceOwnerManager;
        private readonly IDeviceRepositoryManager _deviceRepositoryManager;

        public DeviceOwnerManagementController(IDeviceOwnerManager deviceOwnerManager,
                                               IDeviceRepositoryManager deviceRepositoryManager,
                                               UserManager<AppUser> userManager,
                                               IAdminLogger logger) : base(userManager, logger)
        {
            _deviceOwnerManager = deviceOwnerManager ?? throw new ArgumentNullException(nameof(deviceOwnerManager));
            _deviceRepositoryManager = deviceRepositoryManager ?? throw new ArgumentNullException(nameof(deviceRepositoryManager));
        }

        [OrgAdmin]
        [HttpGet("/api/deviceowners")]
        public Task<ListResponse<DeviceOwnerUserSummary>> GetOwnersAsync() =>
            _deviceOwnerManager.GetOwnersAsync(GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);

        [OrgAdmin]
        [HttpGet("/api/deviceowner/{id}")]
        public Task<InvokeResult<DeviceOwnerUser>> GetOwnerAsync(string id) =>
            _deviceOwnerManager.GetOwnerByIdAsync(id, OrgEntityHeader, UserEntityHeader);

        [OrgAdmin]
        [HttpGet("/api/deviceowner/{id}/devices")]
        public Task<InvokeResult<DeviceOwnerDevices[]>> GetOwnerDevicesAsync(string id) =>
            _deviceOwnerManager.GetDevicesForOwnerAsync(id, OrgEntityHeader, UserEntityHeader);

        [OrgAdmin]
        [HttpPost("/api/deviceowner")]
        public Task<InvokeResult> CreateOwnerAsync([FromBody] DeviceOwnerUser owner) =>
            _deviceOwnerManager.CreateOwnerAsync(owner, OrgEntityHeader, UserEntityHeader);

        [OrgAdmin]
        [HttpPut("/api/deviceowner")]
        public Task<InvokeResult> UpdateOwnerAsync([FromBody] DeviceOwnerUser owner) =>
            _deviceOwnerManager.UpdateOwnerAsync(owner, OrgEntityHeader, UserEntityHeader);

        [OrgAdmin]
        [HttpDelete("/api/deviceowner/{id}")]
        public Task<InvokeResult> DeleteOwnerAsync(string id) =>
            _deviceOwnerManager.DeleteOwnerAsync(id, OrgEntityHeader, UserEntityHeader);

        [OrgAdmin]
        [HttpPost("/api/devices/{deviceRepoId}/device/{deviceId}/owner/{ownerId}")]
        public async Task<InvokeResult> AssignOwnerAsync(string deviceRepoId, string deviceId, string ownerId, bool replaceExisting = false)
        {
            var repo = await _deviceRepositoryManager.GetDeviceRepositoryWithSecretsAsync(deviceRepoId, OrgEntityHeader, UserEntityHeader);
            return await _deviceOwnerManager.AssignOwnerToDeviceAsync(repo, deviceId, ownerId, replaceExisting, OrgEntityHeader, UserEntityHeader);
        }

        [OrgAdmin]
        [HttpDelete("/api/devices/{deviceRepoId}/device/{deviceId}/owner/{ownerId}")]
        public async Task<InvokeResult> RemoveOwnerAsync(string deviceRepoId, string deviceId, string ownerId)
        {
            var repo = await _deviceRepositoryManager.GetDeviceRepositoryWithSecretsAsync(deviceRepoId, OrgEntityHeader, UserEntityHeader);
            return await _deviceOwnerManager.RemoveOwnerFromDeviceAsync(repo, deviceId, ownerId, OrgEntityHeader, UserEntityHeader);
        }

        [OrgAdmin]
        [HttpGet("/api/deviceowner/factory")]
        public DetailResponse<DeviceOwnerUser> CreateOwnerFactory() => DetailResponse<DeviceOwnerUser>.Create();
    }
}
