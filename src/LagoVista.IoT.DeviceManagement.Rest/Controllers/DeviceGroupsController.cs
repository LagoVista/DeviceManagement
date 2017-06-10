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

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    /// <summary>
    /// Device Group Controller
    /// </summary>
    [Authorize]
    public class DeviceGroupsController : LagoVistaBaseController
    {

        IDeviceGroupManager _deviceGroupManager;

        public DeviceGroupsController(IDeviceGroupManager deviceGroupManager, UserManager<LagoVista.UserAdmin.Models.Account.AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _deviceGroupManager = deviceGroupManager;
        }

        /// <summary>
        /// Device Groups - Add
        /// </summary>
        /// <param name="deviceGroup"></param>
        /// <returns></returns>
        [HttpPost("/api/devicegroup")]
        public Task<InvokeResult> AddDeviceGroupAsync([FromBody] DeviceGroup deviceGroup)
        {
            return _deviceGroupManager.AddDeviceGroupAsync(deviceGroup, UserEntityHeader, OrgEntityHeader);
        }

        /// <summary>
        /// Device Groups - Update
        /// </summary>
        /// <param name="deviceGroup"></param>
        /// <returns></returns>
        [HttpPut("/api/devicegroup")]
        public Task<InvokeResult> UpdateDeviceGroupAsync([FromBody] DeviceGroup deviceGroup)
        {
            SetUpdatedProperties(deviceGroup);
            return _deviceGroupManager.UpdateDeviceGroupAsync(deviceGroup, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Groups - Get for Org
        /// </summary>
        /// <param name="orgId">Organization Id</param>
        /// <returns></returns>
        [HttpGet("/api/org/{orgid}/devicegroups")]
        public async Task<ListResponse<DeviceGroupSummary>> GetDeviceGroupsForOrgAsync(String orgId)
        {
            var hostSummaries = await _deviceGroupManager.GetDeviceGroupsForOrgAsync(orgId, UserEntityHeader);
            var response = ListResponse<DeviceGroupSummary>.Create(hostSummaries);

            return response;
        }

        /// <summary>
        /// Device Groups - Add Device to Group
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("/api/devicegroups/add/{groupid}/{deviceid}")]
        public  Task<InvokeResult> AddDeviceToGroupAsync(String groupid, string deviceid)
        {
            return _deviceGroupManager.AddDeviceToGroupAsync(groupid, deviceid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Groups - Remove a Device from a Group
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("/api/devicegroups/remove/{groupid}/{deviceid}")]
        public Task<InvokeResult> RemoveDeviceToGroupAsync(String groupid, string deviceid)
        {
            return _deviceGroupManager.RemoveDeviceFromGroupAsync(groupid, deviceid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Groups - Get Devices for Group
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [HttpGet("/api/devicegroups/{groupid}/devices")]
        public  async Task<ListResponse<EntityHeader>> GetDevicesForGroupAsync(String groupid)
        {
            var devices = await _deviceGroupManager.GetDevicesInGroupAsync(groupid, OrgEntityHeader, UserEntityHeader);

            return ListResponse<EntityHeader>.Create(devices);
        }


        /// <summary>
        /// Device Groups - Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/devicegroups/{id}")]
        public async Task<DetailResponse<DeviceGroup>> GetDeviceGroupAsync(String id)
        {
            var deviceGroup = await _deviceGroupManager.GetDeviceGroupAsync(id, OrgEntityHeader, UserEntityHeader);

            var response = DetailResponse<DeviceGroup>.Create(deviceGroup);

            return response;
        }

        /// <summary>
        /// Device Groups - Key In Use
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/devicegroups/keyinuse/{key}")]
        public Task<bool> QueryKeyInUse(String key)
        {
            return _deviceGroupManager.QueryKeyInUseAsync(key, OrgEntityHeader);
        }

        /// <summary>
        /// Device Groups - Delete
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/api/devicegroups/{id}")]
        public Task<InvokeResult> DeleteDeviceGroupsAsync(string id)
        {
            return _deviceGroupManager.DeleteDeviceGroupAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        ///  Device Groups - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/devicegroups/factory")]
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

