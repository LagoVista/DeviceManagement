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
using LagoVista.IoT.Logging.Loggers;

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

        public DeviceManagementController(IDeviceManager deviceManager, UserManager<LagoVista.UserAdmin.Models.Account.AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
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
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="orgId">Organization Id</param>
        /// <returns></returns>
        [HttpGet("org/{orgid}/{devicerepoid}/devices")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForOrg(String orgId, string devicerepoid)
        {
            //TODO: Need to add paging.
            var devices = await _deviceManager.GetDevicesForOrgIdAsync(devicerepoid, orgId, 0, 100, UserEntityHeader);
            return ListResponse<DeviceSummary>.Create(devices);
        }

        /// <summary>
        /// Device Management - Get For a Location
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="locationid">Organization Id</param>
        /// <returns></returns>
        [HttpGet("location/{locationid}/{devicerepoid}/devices")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForLocationAsync(String locationid, string devicerepoid)
        {
            //TODO: Need to add paging.
            var devices = await _deviceManager.GetDevicesForLocationIdAsync(devicerepoid, locationid, 0, 100, OrgEntityHeader, UserEntityHeader);
            return ListResponse<DeviceSummary>.Create(devices);
        }

        /// <summary>
        /// Device Management - Get By Id
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("device/{devicerepoid}/{id}")]
        public async Task<DetailResponse<Device>> GetDeviceByIdAsync(String id, string devicerepoid)
        {
            var device = await _deviceManager.GetDeviceByIdAsync(devicerepoid, id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<Device>.Create(device);
        }

        /// <summary>
        /// Device Management - Get By DeviceId
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("devicebyid/{devicerepoid}/{deviceid}")]
        public async Task<DetailResponse<Device>> GetDeviceByDeviceId(String deviceid, string devicerepoid)
        {
            var device = await _deviceManager.GetDeviceByDeviceIdAsync(devicerepoid,deviceid, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<Device>.Create(device);
        }

        /// <summary>
        /// Device Management - Check Device Id in Use
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("deviceidinuse/{devicerepoid}/{deviceid}")]
        public Task<DependentObjectCheckResult> GetDeviceIdInUse(string deviceid, string devicerepoid)
        {
            return _deviceManager.CheckIfDeviceIdInUse(devicerepoid, deviceid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Delete
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("device/{id}")]
        public Task<InvokeResult> DeleteDeviceAsync(string id, string devicerepoid)
        {
            return _deviceManager.DeleteDeviceAsync(devicerepoid, id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Create
        /// </summary>
        /// <returns></returns>
        [HttpGet("device/factory/{devicerepoid}")]
        public DetailResponse<Device> CreateDevice(string devicerepoid)
        {
            var response = DetailResponse<Device>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            /* Note we just create it here for now then the record gets inserted we go ahead assign the name */
            response.Model.DeviceRepository = new EntityHeader() { Id = devicerepoid, Text = "TBD" };
            
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
