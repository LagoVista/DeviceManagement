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
using LagoVista.UserAdmin.Models.Users;
using LagoVista.IoT.DeviceManagement.Core;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    /// <summary>
    /// Device Management Services
    /// </summary>
    [Authorize]
    [ConfirmedUser]
    public class DeviceManagementController : LagoVistaBaseController
    {
        IDeviceManager _deviceManager;
        IDeviceRepositoryManager _repoManager;

        public DeviceManagementController(IDeviceRepositoryManager repoManager, IDeviceManager deviceManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _deviceManager = deviceManager;
            _repoManager = repoManager;
        }

        /// <summary>
        /// Device Management - Add New
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        [HttpPost("/api/device/{devicerepoid}")]
        public async Task<InvokeResult> AddDeviceAsync(string devicerepoid, [FromBody] Device device)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.AddDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Update
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        [HttpPut("/api/device/{devicerepoid}")]
        public async Task<InvokeResult> UpdateDeviceAsync(string devicerepoid, [FromBody] Device device)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            SetUpdatedProperties(device);
            return await _deviceManager.UpdateDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Get For Org
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <returns></returns>
        [HttpGet("/api/devices/{devicerepoid}")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForOrg(string devicerepoid)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            //TODO: Need to add paging.
            var devices = await _deviceManager.GetDevicesForOrgIdAsync(repo, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
            return ListResponse<DeviceSummary>.Create(devices);
        }

        /// <summary>
        /// Device Management - Get For a Location
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="locationid">Location Id</param>
        /// <returns></returns>
        [HttpGet("/api/location/{locationid}/devices/{devicerepoid}")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForLocationAsync(string devicerepoid, String locationid)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            //TODO: Need to add paging.
            var devices = await _deviceManager.GetDevicesForLocationIdAsync(repo, locationid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
            return ListResponse<DeviceSummary>.Create(devices);
        }

        /// <summary>
        /// Device Management - Get By Id
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/{id}")]
        public async Task<DetailResponse<Device>> GetDeviceByIdAsync(string devicerepoid, String id)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var device = await _deviceManager.GetDeviceByIdAsync(repo, id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<Device>.Create(device);
        }

        /// <summary>
        /// Device Management - Get By Id
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/{id}/metadata")]
        public async Task<DetailResponse<Device>> GetDeviceByIdAndMetaDataAsync(string devicerepoid, String id)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var device = await _deviceManager.GetDeviceByIdAsync(repo, id, OrgEntityHeader, UserEntityHeader, true);
            return DetailResponse<Device>.Create(device);
        }

        /// <summary>
        /// Device Management - Get By DeviceId
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/deviceid/{deviceid}")]
        public async Task<DetailResponse<Device>> GetDeviceByDeviceId(string devicerepoid, String deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var device = await _deviceManager.GetDeviceByDeviceIdAsync(repo, deviceid, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<Device>.Create(device);
        }

        /// <summary>
        /// Device Management - Get By DeviceId
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/deviceid/{deviceid}/metadata")]
        public async Task<DetailResponse<Device>> GetDeviceByDeviceIdAndMetaData(string devicerepoid, String deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var device = await _deviceManager.GetDeviceByDeviceIdAsync(repo, deviceid, OrgEntityHeader, UserEntityHeader, true);
            return DetailResponse<Device>.Create(device);
        }

        /// <summary>
        /// Device Management - Check Device Id in Use
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/inuse/{deviceid}")]
        public async Task<DependentObjectCheckResult> GetDeviceIdInUse(string devicerepoid, string deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.CheckIfDeviceIdInUse(repo, deviceid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Delete
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/api/device/{devicerepoid}/{id}")]
        public async Task<InvokeResult> DeleteDeviceAsync(string devicerepoid, string id)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.DeleteDeviceAsync(repo, id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Create
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/factory")]
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
        [HttpGet("/api/device/note/factory")]
        public DetailResponse<DeviceNote> CreateDeviceNote()
        {
            var response = DetailResponse<DeviceNote>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);

            return response;
        }
    }
}
