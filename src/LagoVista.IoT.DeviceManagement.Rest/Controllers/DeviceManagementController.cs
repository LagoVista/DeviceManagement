using LagoVista.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using LagoVista.Core;
using Microsoft.AspNetCore.Mvc;
using LagoVista.Core.Validation;
using System.Threading.Tasks;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.Models;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    /// <summary>
    /// Device Management Services
    /// </summary>
    [Authorize]
    [ConfirmedUser]
    [Route("api")]
    public class DeviceManagementController : LagoVistaBaseController
    {
        IDeviceManager _deviceManager;

        public DeviceManagementController(IDeviceManager deviceManager, UserManager<LagoVista.UserAdmin.Models.Account.AppUser> userManager, LagoVista.Core.PlatformSupport.ILogger logger) : base(userManager, logger)
        {
            _deviceManager = deviceManager;
        }

        /// <summary>
        /// Device Management - Add New
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [HttpPost("device")]
        public Task<InvokeResult> AddDeviceAsync([FromBody] Device device)
        {
            return _deviceManager.AddDeviceAsync(device, UserEntityHeader, OrgEntityHeader);
        }

        /// <summary>
        /// Device Management - Update
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [HttpPut("device")]
        public Task<InvokeResult> UpdateDeviceAsync([FromBody] Device device)
        {
            SetUpdatedProperties(device);
            return _deviceManager.UpdateDeviceAsync(device, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Get For Org
        /// </summary>
        /// <param name="orgId">Organization Id</param>
        /// <returns></returns>
        [HttpGet("org/{orgid}/devices")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForOrg(String orgId)
        {
            //TODO: Need to add paging.
            var devices = await _deviceManager.GetDevicesForOrgIdAsync(orgId, 0, 100, UserEntityHeader);
            return ListResponse<DeviceSummary>.Create(devices);
        }

        /// <summary>
        /// Device Management - Get For a Location
        /// </summary>
        /// <param name="locationid">Organization Id</param>
        /// <returns></returns>
        [HttpGet("location/{locationid}/devices")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForLocationAsync(String locationid)
        {
            //TODO: Need to add paging.
            var devices = await _deviceManager.GetDevicesForLocationIdAsync(locationid, 0, 100, OrgEntityHeader, UserEntityHeader);
            return ListResponse<DeviceSummary>.Create(devices);
        }

        /// <summary>
        /// Device Management - Get By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("device/{id}")]
        public async Task<DetailResponse<Device>> GetDeviceByIdAsync(String id)
        {
            var device = await _deviceManager.GetDeviceByIdAsync(id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<Device>.Create(device);
        }

        /// <summary>
        /// Device Management - Get By DeviceId
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("devicebyid/{deviceid}")]
        public async Task<DetailResponse<Device>> GetDeviceByDeviceId(String deviceid)
        {
            var device = await _deviceManager.GetDeviceByDeviceIdAsync(deviceid, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<Device>.Create(device);
        }

        /// <summary>
        /// Device Management - Check Device Id in Use
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("deviceidinuse/{deviceid}")]
        public Task<DependentObjectCheckResult> GetDeviceIdInUse(string deviceid)
        {
            return _deviceManager.CheckIfDeviceIdInUse(deviceid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Delete
        /// </summary>
        /// <returns></returns>
        [HttpDelete("device/{id}")]
        public Task<InvokeResult> DeleteDeviceAsync(string id)
        {
            return _deviceManager.DeleteDeviceAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Create
        /// </summary>
        /// <returns></returns>
        [HttpGet("device/factory")]
        public DetailResponse<Device> CreateDevice()
        {
            var response = DetailResponse<Device>.Create();
            response.Model.Id = Guid.NewGuid().ToId();

            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }

        /// <summary>
        /// Device Management - Create
        /// </summary>
        /// <returns></returns>
        [HttpGet("device/notes/factory")]
        public DetailResponse<DeviceNote> CreateDeviceNote()
        {
            var response = DetailResponse<DeviceNote>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);

            return response;
        }
    }
}
