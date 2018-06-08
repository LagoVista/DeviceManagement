using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.Models.Geo;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core;
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
    /// <summary>
    /// Device Management Services
    /// </summary>
    [Authorize]
    [ConfirmedUser]
    public class DeviceManagementController : LagoVistaBaseController
    {
        IDeviceManager _deviceManager;
        IDeviceRepositoryManager _repoManager;
        IConsoleWriter _console;

        public DeviceManagementController(IDeviceRepositoryManager repoManager, IDeviceManager deviceManager, UserManager<AppUser> userManager, IAdminLogger logger, IConsoleWriter console) : base(userManager, logger)
        {
            _deviceManager = deviceManager;
            _repoManager = repoManager;
            _console = console;
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
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
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
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            SetUpdatedProperties(device);
            return await _deviceManager.UpdateDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Get devices for a device repository
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <returns></returns>
        [HttpGet("/api/devices/{devicerepoid}")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForDeviceRepo(string devicerepoid)
        {
            _console.WriteLine("DeviceManagementController.GetDevicesForDeviceRepo()");
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GetDevicesForDeviceRepoAsync(repo, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Get For a Location
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="locationid">Location Id</param>
        /// <returns></returns>
        [HttpGet("/api/location/{locationid}/devices/{devicerepoid}")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForLocationAsync(string devicerepoid, string locationid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GetDevicesForLocationIdAsync(repo, locationid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// Device Management - Get Full Devices by Config Id
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="configid">Configuration Id</param>
        /// <returns></returns>
        [HttpGet("/api/devices/{devicerepoid}/deviceconfig/{configid}/full")]
        public async Task<ListResponse<Device>> GetFullDevicesForConfigAsync(string devicerepoid, string configid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GetFullDevicesWithConfigurationAsync(repo, configid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Get Devices by Config Id
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="configid">Configuration Id</param>
        /// <returns></returns>
        [HttpGet("/api/devices/{devicerepoid}/deviceconfig/{configid}")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForConfigAsync(string devicerepoid, string configid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GetDevicesWithConfigurationAsync(repo, configid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Get Devices by Type Id
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="devicetypeid">Device Type Id</param>
        /// <returns></returns>
        [HttpGet("/api/devices/{devicerepoid}/devicetype/{devicetypeid}")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForDeviceTypeAsync(string devicerepoid, string devicetypeid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GetDevicesWithDeviceTypeAsync(repo, devicetypeid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Search for devices by device id
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="search">Configuration Id</param>
        /// <returns></returns>
        [HttpGet("/api/devices/{devicerepoid}/search/{search}")]
        public async Task<ListResponse<DeviceSummary>> SearchDeviceAsync(string devicerepoid, string search)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.SearchByDeviceIdAsync(repo, search, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Get Devices In Status
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="status">Primary Device Status</param>
        /// <returns></returns>
        [HttpGet("/api/devices/{devicerepoid}/status/{status}")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesInStatusAsync(string devicerepoid, string status)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GetDevicesInStatusAsync(repo, status, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// Device Management - Get Devices In Custom Status
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="customstatus">Custom Status for Device</param>
        /// <returns></returns>
        [HttpGet("/api/devices/{devicerepoid}/customstatus/{customstatus}")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesInCustomStatusConfigAsync(string devicerepoid, string customstatus)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GetDevicesInCustomStatusAsync(repo, customstatus, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// Device Management - Get By Id
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/{id}")]
        public async Task<DetailResponse<Device>> GetDeviceByIdAsync(string devicerepoid, string id)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var device = await _deviceManager.GetDeviceByIdAsync(repo, id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<Device>.Create(device);
        }

        /// <summary>
        /// Device Management - Update Status (primary status of device)
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="id">Unique id of device</param>
        /// <param name="status">Status of device, (not case sensitive) see StatusTypes for device object.</param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/{id}/status/{status}")]
        public async Task<InvokeResult> UpdateDeviceStatusAsync(string devicerepoid, string id, string status)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.UpdateDeviceStatusAsync(repo, id,status, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Update Status (custom status of device)
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="id">Unique id of device</param>
        /// <param name="status">Status of device, (not case sensitive) see StatusTypes for device object.</param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/{id}/customstatus/{status}")]
        public async Task<InvokeResult> UpdateDeviceCustomStatusAsync(string devicerepoid, string id, string status)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.UpdateDeviceCustomStatusAsync(repo, id, status, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Get By Id
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/{id}/metadata")]
        public async Task<DetailResponse<Device>> GetDeviceByIdAndMetaDataAsync(string devicerepoid, string id)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
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
        public async Task<DetailResponse<Device>> GetDeviceByDeviceId(string devicerepoid, string deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
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
        public async Task<DetailResponse<Device>> GetDeviceByDeviceIdAndMetaData(string devicerepoid, string deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
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
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
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
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
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

        /// <summary>
        /// Device Management - Add Note to Device
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="id">Unique id of device</param>
        /// <param name="deviceNote"></param>
        /// <returns></returns>
        [HttpPost("/api/device/{devicerepoid}/{deviceid}/note")]
        public async Task<InvokeResult> AddNoteAsync(string devicerepoid, string id, [FromBody] DeviceNote deviceNote)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.AddNoteAsync(repo, id, deviceNote, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Add Note to Device
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="id">Unique id of device</param>
        /// <param name="geolocation"></param>
        /// <returns></returns>
        [HttpPost("/api/device/{devicerepoid}/{deviceid}/geolocation")]
        public async Task<InvokeResult> UpdateGeoLocationAsync(string devicerepoid, string id, [FromBody] GeoLocation geolocation)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.UpdateGeoLocationAsync(repo, id, geolocation, OrgEntityHeader, UserEntityHeader);
        }
    }
}
