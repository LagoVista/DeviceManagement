// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c78841f39f575aa476d92b967cc5d41cd18e3f5f5ea7b81db11466c93d7628ec
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    /// <summary>
    /// Client API Device Group Controller
    /// </summary>
    [Authorize(AuthenticationSchemes = "APIToken")]
    [ConfirmedUser]
    public class AppsDeviceGroupController : ClientAppAPIController
    {

        IDeviceGroupManager _deviceGroupManager;

        public AppsDeviceGroupController(IDeviceRepositoryManager repoManager,
            IDeviceGroupManager deviceGroupManager, UserManager<AppUser> userManager, IAdminLogger logger)
            : base(repoManager, userManager, logger)
        {
            _deviceGroupManager = deviceGroupManager;
        }

        /// <summary>
        /// Client API Device Groups - Add
        /// </summary>
        /// <param name="deviceGroup"></param>
        /// <returns></returns>
        [HttpPost("/clientapi/repo/group")]
        public async Task<InvokeResult> AddDeviceGroupAsync([FromBody] DeviceGroup deviceGroup)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceGroupManager.AddDeviceGroupAsync(repo, deviceGroup, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Groups - Update
        /// </summary>
        /// <param name="deviceGroup"></param>
        /// <returns></returns>
        [HttpPut("/clientapi/repo/group")]
        public async Task<InvokeResult> UpdateDeviceGroupAsync([FromBody] DeviceGroup deviceGroup)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            SetUpdatedProperties(deviceGroup);
            return await _deviceGroupManager.UpdateDeviceGroupAsync(repo, deviceGroup, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Groups - Get for Current Org
        /// </summary>
        /// <returns></returns>
        [HttpGet("/clientapi/repo/groups")]
        public async Task<ListResponse<DeviceGroupSummary>> GetDeviceGroupsForOrgAsync(string devicerepoid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            var deviceGroupSummaries = await _deviceGroupManager.GetDeviceGroupsForOrgAsync(repo, OrgEntityHeader.Id, UserEntityHeader);
            var response = ListResponse<DeviceGroupSummary>.Create(deviceGroupSummaries);

            return response;
        }

        /// <summary>
        /// Client API Device Groups - Add Device to Group
        /// </summary>

        /// <param name="groupid"></param>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("/clientapi/repo/group/{groupid}/add/{deviceid}")]
        public async Task<InvokeResult<DeviceGroupEntry>> AddDeviceToGroupAsync(string groupid, string deviceid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceGroupManager.AddDeviceToGroupAsync(repo, groupid, deviceid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Groups - Remove a Device from a Group
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("/clientapi/repo/group/{groupid}/remove/{deviceid}")]
        public async Task<InvokeResult> RemoveDeviceToGroupAsync(string groupid, string deviceid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceGroupManager.RemoveDeviceFromGroupAsync(repo, groupid, deviceid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Groups - Get Devices for Group
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [HttpGet("/clientapi/repo/group/{groupid}/devices")]
        public async Task<ListResponse<EntityHeader>> GetDevicesForGroupAsync(string groupid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            var devices = await _deviceGroupManager.GetDevicesInGroupAsync(repo, groupid, OrgEntityHeader, UserEntityHeader);

            return ListResponse<EntityHeader>.Create(devices);
        }


        /// <summary>
        /// Client API Device Groups - Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/clientapi/repo/group/{id}")]
        public async Task<DetailResponse<DeviceGroup>> GetDeviceGroupAsync(string id)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            var deviceGroup = await _deviceGroupManager.GetDeviceGroupAsync(repo, id, OrgEntityHeader, UserEntityHeader);

            return DetailResponse<DeviceGroup>.Create(deviceGroup);
        }

        /// <summary>
        /// Client API Device Groups - Key In Use
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("/clientapi/repo/groups/keyinuse/{key}")]
        public async Task<bool> QueryKeyInUse(String key)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceGroupManager.QueryKeyInUseAsync(repo, key, OrgEntityHeader);
        }

        /// <summary>
        /// Client API Device Groups - Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/clientapi/repo/group/{id}")]
        public async Task<InvokeResult> DeleteDeviceGroupsAsync(string id)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceGroupManager.DeleteDeviceGroupAsync(repo, id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        ///  Device Groups - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/clientapi/repo/group/factory")]
        public DetailResponse<DeviceGroup> CreateDeviceGroup()
        {
            var response = DetailResponse<DeviceGroup>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }

        /// <summary>
        ///  Device Groups - Get Summary Level Data from Devices
        /// </summary>
        /// <returns></returns>
        [HttpGet("/clientapi/repo/group/{groupid}/devices/summarydata")]
        public async Task<ListResponse<DeviceSummaryData>> GetGroupDevicesSummaryData(string groupid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceGroupManager.GetDeviceGroupSummaryDataAsync(repo, groupid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }
    }
}

