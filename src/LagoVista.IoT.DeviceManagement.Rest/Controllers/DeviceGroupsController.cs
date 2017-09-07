using LagoVista.Core.Interfaces;
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
using LagoVista.IoT.DeviceManagement.Core;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    /// <summary>
    /// Device Group Controller
    /// </summary>
    [Authorize]
    public class DeviceGroupsController : LagoVistaBaseController
    {

        IDeviceGroupManager _deviceGroupManager;
        IDeviceRepositoryManager _repoManager;

        public DeviceGroupsController(IDeviceRepositoryManager repoManager, IDeviceGroupManager deviceGroupManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _deviceGroupManager = deviceGroupManager;
            _repoManager = repoManager;
        }

        /// <summary>
        /// Device Groups - Add
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="deviceGroup"></param>
        /// <returns></returns>
        [HttpPost("/api/device/{devicerepoid}/group")]
        public async Task<InvokeResult> AddDeviceGroupAsync(string devicerepoid, [FromBody] DeviceGroup deviceGroup)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceGroupManager.AddDeviceGroupAsync(repo, deviceGroup, UserEntityHeader, OrgEntityHeader);
        }

        /// <summary>
        /// Device Groups - Update
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="deviceGroup"></param>
        /// <returns></returns>
        [HttpPut("/api/device/{devicerepoid}/group")]
        public async Task<InvokeResult> UpdateDeviceGroupAsync(string devicerepoid, [FromBody] DeviceGroup deviceGroup)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            SetUpdatedProperties(deviceGroup);
            return await _deviceGroupManager.UpdateDeviceGroupAsync(repo, deviceGroup, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Groups - Get for Current Org
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/groups")]
        public async Task<ListResponse<DeviceGroupSummary>> GetDeviceGroupsForOrgAsync(string devicerepoid)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var deviceGroupSummaries = await _deviceGroupManager.GetDeviceGroupsForOrgAsync(repo, OrgEntityHeader.Id, UserEntityHeader);
            var response = ListResponse<DeviceGroupSummary>.Create(deviceGroupSummaries);

            return response;
        }

        /// <summary>
        /// Device Groups - Add Device to Group
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="groupid"></param>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/groups/{groupid}/{deviceid}/add")]
        public  async Task<InvokeResult> AddDeviceToGroupAsync(string devicerepoid, String groupid, string deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceGroupManager.AddDeviceToGroupAsync(repo, groupid, deviceid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Groups - Remove a Device from a Group
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="groupid"></param>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/groups/{groupid}/{deviceid}/remove")]
        public async Task<InvokeResult> RemoveDeviceToGroupAsync(string devicerepoid, String groupid, string deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceGroupManager.RemoveDeviceFromGroupAsync(repo, groupid, deviceid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Groups - Get Devices for Group
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/groups/{groupid}/devices")]
        public  async Task<ListResponse<EntityHeader>> GetDevicesForGroupAsync(string devicerepoid, String groupid)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var devices = await _deviceGroupManager.GetDevicesInGroupAsync(repo, groupid, OrgEntityHeader, UserEntityHeader);

            return ListResponse<EntityHeader>.Create(devices);
        }


        /// <summary>
        /// Device Groups - Get
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/groups/{id}")]
        public async Task<DetailResponse<DeviceGroup>> GetDeviceGroupAsync(string devicerepoid, String id)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var deviceGroup = await _deviceGroupManager.GetDeviceGroupAsync(repo, id, OrgEntityHeader, UserEntityHeader);

            return  DetailResponse<DeviceGroup>.Create(deviceGroup);
        }

        /// <summary>
        /// Device Groups - Key In Use
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/groups/keyinuse/{key}")]
        public async Task<bool> QueryKeyInUse(string devicerepoid, String key)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceGroupManager.QueryKeyInUseAsync(repo, key, OrgEntityHeader);
        }

        /// <summary>
        /// Device Groups - Delete
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/api/device/{devicerepoid}/groups/{id}")]
        public async Task<InvokeResult> DeleteDeviceGroupsAsync(string devicerepoid, string id)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceGroupManager.DeleteDeviceGroupAsync(repo, id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        ///  Device Groups - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/groups/factory")]
        public DetailResponse<DeviceGroup> CreateDeviceGroup()
        {
            var response = DetailResponse<DeviceGroup>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }
    }
}

