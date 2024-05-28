using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.Models.Geo;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.MediaServices.Models;
using LagoVista.UserAdmin.Interfaces.Managers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IDeviceManager _deviceManager;
        private readonly IOrganizationManager _orgManager;
        private readonly IDeviceRepositoryManager _repoManager;
        private readonly UserManager<AppUser> _userManager;

        public DeviceManagementController(IDeviceRepositoryManager repoManager, IDeviceManager deviceManager, IOrganizationManager orgManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _orgManager = orgManager;
            _deviceManager = deviceManager;
            _repoManager = repoManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Device Management - Add New
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="device"></param>
        /// <param name="reassign"></param>
        /// <returns></returns>
        [HttpPost("/api/device/{devicerepoid}")]
        public async Task<InvokeResult<Device>> AddDeviceAsync(string devicerepoid, [FromBody] Device device, bool reassign = false)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.AddDeviceAsync(repo, device, reassign, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Add an image to device.
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="devicerepoid"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("/api/device/{devicerepoid}/device/{deviceid}/image")]
        public async Task<InvokeResult<MediaResource>> AddDeviceImage(string devicerepoid, string deviceid, [FromForm] IFormFile file)
        {
            using (var strm = file.OpenReadStream())
            {
                var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
                return await _deviceManager.AddDeviceImageAsync(repo, deviceid, strm, file.FileName, file.ContentType, OrgEntityHeader, UserEntityHeader);
            }
        }

        /// <summary>
        /// Device Management - Add an image to device.
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="devicerepoid"></param>
        /// <param name="sensor"></param>
        /// <returns></returns>
        [HttpPost("/api/device/{devicerepoid}/device/{deviceid}/sensor")]
        public async Task<InvokeResult<List<Sensor>>> UpdateSensor(string devicerepoid, string deviceid, [FromBody] Sensor sensor)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var device = await _deviceManager.GetDeviceByIdAsync(repo, deviceid, OrgEntityHeader, UserEntityHeader);
            var existingSensor = device.SensorCollection.SingleOrDefault(snsr => snsr.Id == sensor.Id);
            if (existingSensor != null)
            {
                device.SensorCollection.Remove(existingSensor);
            }

            device.SensorCollection.Add(sensor);

            var updateResult = await _deviceManager.UpdateDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);
            if (updateResult.Successful)
                return InvokeResult<List<Sensor>>.Create(device.SensorCollection);
            else
                return InvokeResult<List<Sensor>>.FromInvokeResult(updateResult);
        }

        /// <summary>
        /// Device Management - Add an image to device.
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="devicerepoid"></param>
        /// <param name="sensorid"></param>
        /// <returns></returns>
        [HttpDelete("/api/device/{devicerepoid}/device/{deviceid}/sensor/{sensorid}")]
        public async Task<InvokeResult<List<Sensor>>> DeleteSensor(string devicerepoid, string deviceid, string sensorid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var device = await _deviceManager.GetDeviceByIdAsync(repo, deviceid, OrgEntityHeader, UserEntityHeader);
            var existingSensor = device.SensorCollection.Single(snsr => snsr.Id == sensorid);
            if (existingSensor != null)
            {
                device.SensorCollection.Remove(existingSensor);
            }

            var updateResult = await _deviceManager.UpdateDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);
            if (updateResult.Successful)
                return InvokeResult<List<Sensor>>.Create(device.SensorCollection);
            else
                return InvokeResult<List<Sensor>>.FromInvokeResult(updateResult);
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
        /// Device Management - Update
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="id"></param>
        /// <param name="macaddress"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/{id}/macaddress/{macaddress}/set")]
        public async Task<InvokeResult> UpdateDeviceMacAddressAsync(string devicerepoid, string id, string macaddress)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.UpdateDeviceMacAddressAsync(repo, id, macaddress, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Update
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="id"></param>
        /// <param name="iosbleaddress"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/{id}/iosbleaddress/{iosbleaddress}/set")]
        public async Task<InvokeResult> UpdateDeviceiOSBLEAddressAsync(string devicerepoid, string id, string iosbleaddress)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.UpdateDeviceiOSBleAddressAsync(repo, id, iosbleaddress, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Get Device by MacAddress (mainly used to check if it's already registered)
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="macaddress"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/macaddress/{macaddress}")]
        public async Task<InvokeResult<Device>> GetDeviceByMacAddress(string devicerepoid, string macaddress)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GetDeviceByMacAddressAsync(repo, macaddress, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Get Device by MacAddress (mainly used to check if it's already registered)
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="iosbleaddress"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/iosbleaddress/{iosbleaddress}")]
        public async Task<InvokeResult<Device>> GetDeviceByiOSBLEAddress(string devicerepoid, string iosbleaddress)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GetDeviceByiOSBLEAddressAsync(repo, iosbleaddress, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Download an image.        
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="deviceid"></param>
        /// <param name="mediaid"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/device/{deviceid}/image/{mediaid}")]
        public async Task<IActionResult> DownloadMediaAsync(string devicerepoid, string deviceid, string mediaid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);

            var response = await _deviceManager.GetDeviceImageAsync(repo, deviceid, mediaid, OrgEntityHeader, UserEntityHeader);

            var ms = new MemoryStream(response.ImageBytes);
            return new FileStreamResult(ms, response.ContentType);
        }

        /// <summary>
        /// Device Management - Get devices for a device repository
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <returns></returns>
        [HttpGet("/api/devices/{devicerepoid}")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForDeviceRepo(string devicerepoid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GetDevicesForDeviceRepoAsync(repo, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Get devices for a device repository that are assigned to a user.
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="userid">User Id to filter devices</param>
        /// <returns></returns>
        [HttpGet("/api/devices/{devicerepoid}/{userid}")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForDeviceRepoForUserAsync(string devicerepoid, string userid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GetDevicesForDeviceRepoForUserAsync(repo, userid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
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
        /// Device Management - Get Device Children
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="parentid">Parent Device Idd</param>
        /// <returns></returns>
        [HttpGet("/api/devices/{devicerepoid}/{parentid}/children")]
        public async Task<ListResponse<DeviceSummary>> GetDeviceChildren(string devicerepoid, string parentid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GetChildDevicesAsync(repo, parentid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Get Device Children
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="parentid">Parent Device Id</param>
        /// <param name="childid">Child Device Id to Attach</param>
        /// <returns></returns>
        [HttpGet("/api/devices/{devicerepoid}/{parentid}/attachchild/{childid}")]
        public async Task<InvokeResult> AttachChildDeviceAsync(string devicerepoid, string parentid, string childid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.AttachChildDeviceAsync(repo, parentid, childid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Get Device Children
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="parentid">Parent Device Id</param>
        /// <param name="childid">Child Device Id to be removed</param>
        /// <returns></returns>
        [HttpGet("/api/devices/{devicerepoid}/{parentid}/removechild/{childid}")]
        public async Task<InvokeResult> RemoveChildDeviceAsync(string devicerepoid, string parentid, string childid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.RemoveChildDeviceAsync(repo, parentid, childid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
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
        /// <param name="id">Unique id of the device</param>
        /// <returns></returns>
        [HttpGet("/api/devices/{devicerepoid}/{id}/reset")]
        public async Task<InvokeResult> ResetDeviceDataAsync(string devicerepoid, string id)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.ClearDeviceDataAsync(repo, id, OrgEntityHeader, UserEntityHeader);
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
            var response = DetailResponse<Device>.Create(device);
            response.SaveUrl = response.SaveUrl.Replace("{devicerepoid}", devicerepoid);
            response.InsertUrl = response.InsertUrl.Replace("{devicerepoid}", devicerepoid);
            response.UpdateUrl = response.UpdateUrl.Replace("{devicerepoid}", devicerepoid);
            return response;
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
            return await _deviceManager.UpdateDeviceStatusAsync(repo, id, status, OrgEntityHeader, UserEntityHeader);
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
            var form = DetailResponse<Device>.Create(device);

            form.ModelTitle = device.DeviceLabel;
            form.View[nameof(device.DeviceId).CamelCase()].Label = device.DeviceIdLabel;
            form.View[nameof(device.Name).CamelCase()].Label = device.DeviceNameLabel;
            form.View[nameof(device.DeviceType).CamelCase()].Label = device.DeviceTypeLabel;
            form.SaveUrl = form.SaveUrl.Replace("{devicerepoid}", devicerepoid);
            form.InsertUrl = form.InsertUrl.Replace("{devicerepoid}", devicerepoid);
            form.UpdateUrl = form.UpdateUrl.Replace("{devicerepoid}", devicerepoid);

            return form;
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
            var form = DetailResponse<Device>.Create(device);

            form.ModelTitle = device.DeviceLabel;
            form.View[nameof(device.DeviceId).CamelCase()].Label = device.DeviceIdLabel;
            form.View[nameof(device.Name).CamelCase()].Label = device.DeviceNameLabel;
            form.View[nameof(device.DeviceType).CamelCase()].Label = device.DeviceTypeLabel;
            form.SaveUrl = form.SaveUrl.Replace("{devicerepoid}", devicerepoid);
            form.InsertUrl = form.InsertUrl.Replace("{devicerepoid}", devicerepoid);
            form.UpdateUrl = form.UpdateUrl.Replace("{devicerepoid}", devicerepoid);

            return form;
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
            var form = DetailResponse<Device>.Create(device);

            form.ModelTitle = device.DeviceLabel;
            form.View[nameof(device.DeviceId).CamelCase()].Label = device.DeviceIdLabel;
            form.View[nameof(device.Name).CamelCase()].Label = device.DeviceNameLabel;
            form.View[nameof(device.DeviceType).CamelCase()].Label = device.DeviceTypeLabel;
            form.SaveUrl = form.SaveUrl.Replace("{devicerepoid}", devicerepoid);
            form.InsertUrl = form.InsertUrl.Replace("{devicerepoid}", devicerepoid);
            form.UpdateUrl = form.UpdateUrl.Replace("{devicerepoid}", devicerepoid);

            return form;
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
        /// Device Management - Get device connection log
        /// </summary>
        /// <param name="devicerepoid">Device Repo Id</param>
        /// <param name="deviceid">Device Id</param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/{deviceid}/connectionlog")]
        public async Task<ListResponse<DeviceConnectionEvent>> GetDeviceConnectionEventsAsync(string devicerepoid, string deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GetConnectionEventsForDeviceAsync(repo, deviceid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
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
            response.SaveUrl = response.SaveUrl.Replace("{devicerepoid}", devicerepoid);
            response.InsertUrl = response.InsertUrl.Replace("{devicerepoid}", devicerepoid);
            response.UpdateUrl = response.UpdateUrl.Replace("{devicerepoid}", devicerepoid);
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }

        /// <summary>
        /// Device Management - Change Device Id
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="id">id</param>
        /// <param name="newdeviceid">new device id</param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/{id}/set/deviceid/{newdeviceid}")]
        public async Task<InvokeResult<Device>> ChangeDeviceIdAsync(string devicerepoid, string id, string newdeviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var existingDevice = _deviceManager.GetDeviceByDeviceIdAsync(repo, newdeviceid, OrgEntityHeader, UserEntityHeader);
            if (existingDevice != null)
                return InvokeResult<Device>.FromError($"The device id {newdeviceid} is already assigned to a different device, device ids must be unique.");

            var device = await _deviceManager.GetDeviceByIdAsync(repo, id, OrgEntityHeader, UserEntityHeader);
            device.DeviceId = newdeviceid;
            await _deviceManager.UpdateDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);
            return InvokeResult<Device>.Create(device);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="devicetypeid"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/{devicetypeid}/create")]
        public async Task<InvokeResult<Device>> CreateDeviceAsync(string devicerepoid, string devicetypeid)
        {
            var deviceRepo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.CreateDeviceAsync(deviceRepo, devicetypeid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="devicetypekey"></param>
        /// <returns></returns>
        [HttpGet("/api/device/{devicerepoid}/key/{devicetypekey}/create")]
        public async Task<InvokeResult<Device>> CreateDeviceForDeviceKeyAsync(string devicerepoid, string devicetypekey)
        {
            var deviceRepo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.CreateDeviceForDeviceKeyAsync(deviceRepo, devicetypekey, OrgEntityHeader, UserEntityHeader);
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
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="deviceid">Unique id of device</param>
        /// <param name="deviceNote"></param>
        /// <returns></returns>
        [HttpPost("/api/device/{devicerepoid}/{deviceid}/note")]
        public async Task<InvokeResult> AddNoteAsync(string devicerepoid, string deviceid, [FromBody] DeviceNote deviceNote)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.AddNoteAsync(repo, deviceid, deviceNote, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Set 
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="id">Unique id of device</param>
        /// <param name="geolocation"></param>
        /// <returns></returns>
        [HttpPost("/api/device/{devicerepoid}/{deviceid}/geolocation")]
        public async Task<InvokeResult> UpdateGeoLocationAsync(string devicerepoid, string id, [FromBody] GeoLocation geolocation)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.UpdateGeoLocationAsync(repo, id, geolocation, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Clear device error code
        /// </summary>  
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="deviceid">Unique id of device</param>
        /// <param name="errorcode">Error code to clear</param>
        /// <returns></returns>
        [HttpDelete("/api/device/{devicerepoid}/{deviceid}/error/{errorcode}")]
        public async Task<InvokeResult> ClearErrorCodeAsync(string devicerepoid, string deviceid, string errorcode)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.ClearDeviceErrorAsync(repo, deviceid, errorcode, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Attach WiFi Connection Profile
        /// </summary>
        /// <param name="devicerepoid"></param>
        /// <param name="deviceid"></param>
        /// <param name="connectionProfile"></param>
        /// <returns></returns>
        [HttpPost("/api/device/{devicerepoid}/{deviceid}/wificonnection")]
        public async Task<InvokeResult> UpdateWiFiConnectionProfileForDeviceAsync(string devicerepoid, string deviceid, [FromBody] EntityHeader connectionProfile)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var device = await _deviceManager.GetDeviceByIdAsync(repo, deviceid, OrgEntityHeader, UserEntityHeader);
            device.WiFiConnectionProfile = connectionProfile;
            await _deviceManager.UpdateDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);
            return InvokeResult.Success;
        }

        /// <summary>
        /// Device Management - Create a new empty sensor that can be attached to a device.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/device/sensor/factory")]
        public DetailResponse<Sensor> CreateSensor()
        {
            return DetailResponse<Sensor>.Create();
        }


        /// <summary>
        /// Device Management - Create a new empty sensor that can be attached to a device.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/device/sensordefinition/factory")]
        public DetailResponse<SensorDefinition> CreateSensorConfiguration()
        {
            return DetailResponse<SensorDefinition>.Create();
        }

        [HttpPut("/api/device/{devicerepoid}/{deviceid}/property")]
        public async Task<InvokeResult> UpdateDevicePropertyAsync(string devicerepoid, string deviceid, [FromBody] AttributeValue value)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var device = await _deviceManager.GetDeviceByIdAsync(repo, deviceid, OrgEntityHeader, UserEntityHeader, false);

            var attrValue = device.Properties.FirstOrDefault(prop => prop.Key == value.Key);
            if (attrValue == null)
            {
                value.Value = value.Value;
                value.LastUpdated = DateTime.UtcNow.ToJSONString();
                device.Properties.Add(value);
            }
            else
            {
                attrValue.Value = value.Value;
                attrValue.LastUpdated = DateTime.UtcNow.ToJSONString();
                attrValue.LastUpdatedBy = UserEntityHeader.Text;
            }



            return await _deviceManager.UpdateDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);
        }


        [HttpGet("/api/device/{devicerepoid}/{id}/pin/set/{pin}")]
        public async Task<InvokeResult<string>> CreateDeviceLink(string devicerepoid, string id, string pin)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GenerateSecureDeviceLinkAsync(repo, id, pin, OrgEntityHeader, UserEntityHeader);
        }


        [HttpGet("/api/device/{devicerepoid}/{deviceid}/location/{locationid}/add")]
        public async Task<InvokeResult> AddDeviceToLocationAsync(string devicerepoid, string deviceid, string locationid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.AddDeviceToLocationAsync(repo, deviceid, locationid, OrgEntityHeader, UserEntityHeader);
        }

        [HttpDelete("/api/device/{devicerepoid}/{deviceid}/location/{locationid}/remove")]
        public async Task<InvokeResult> RemoveDeviceToLocationAsync(string devicerepoid, string deviceid, string locationid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.RemoveDeviceFromLocation(repo, deviceid, locationid, OrgEntityHeader, UserEntityHeader);
        }

        [HttpPost("/api/device/{devicerepoid}/{deviceid}/sensor")]
        public async Task<InvokeResult> SetSensorAsync(string devicerepoid, string deviceid, [FromBody] Sensor sensor)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            var device = await _deviceManager.GetDeviceByIdAsync(repo, deviceid, OrgEntityHeader, UserEntityHeader, false);
            if (device.SensorCollection == null)
                device.SensorCollection = new System.Collections.Generic.List<Sensor>();

            var existingSensor = device.SensorCollection.Where(snsr => snsr.Technology.HasValue && snsr.Technology == sensor.Technology && snsr.PortIndex.HasValue && snsr.PortIndex == sensor.PortIndex).FirstOrDefault();
            if (existingSensor != null)
                device.SensorCollection.Remove(existingSensor);

            device.SensorCollection.Add(sensor);

            return await _deviceManager.UpdateDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);
        }

        [AllowAnonymous]
        [HttpGet("/api/device/{orgid}/{devicerepoid}/{id}/{pin}/view")]
        public async Task<InvokeResult<Device>> GetDeviceAsync(string orgId, string devicerepoid, string id, string pin)
        {
            var org = EntityHeader.Create(orgId, "PIN Device Access");
            var user = EntityHeader.Create(Guid.Empty.ToId(), "PIN Device Access");
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, org, user);

            return InvokeResult<Device>.Create(await _deviceManager.GetDeviceByIdWithPinAsync(repo, id, pin, org, user, true));
        }

        /// <summary>
        /// Device Management - Add Note to Device
        /// </summary>
        /// <param name="devicerepoid">Device Repository Id</param>
        /// <param name="newuser">A new user to be added as a device</param>
        /// <param name="overwrite"></param>
        /// <returns>App User</returns>
        [OrgAdmin]
        [HttpPost("/api/device/{devicerepoid}/userdevice")]
        public async Task<InvokeResult<AppUser>> AddDeviceUser(string devicerepoid, [FromBody] DeviceUserRegistrationRequest newuser, bool overwrite = false)
        {
            String userId = Guid.NewGuid().ToId();

            newuser.Device.OwnerUser = EntityHeader.Create(userId, newuser.Email);

            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(newuser.Device.DeviceRepository.Id, OrgEntityHeader, UserEntityHeader);

            var addDeviceResult = await _deviceManager.AddDeviceAsync(repo, newuser.Device, overwrite, OrgEntityHeader, UserEntityHeader);
            if (!addDeviceResult.Successful)
            {
                return InvokeResult<AppUser>.FromInvokeResult(addDeviceResult.ToInvokeResult());
            }

            var currentOrg = await _orgManager.GetOrganizationAsync(OrgEntityHeader.Id, OrgEntityHeader, UserEntityHeader);

            var appUser = new AppUser()
            {
                Id = userId,
                FirstName = newuser.FirstName,
                LastName = newuser.LastName,
                CurrentOrganization = currentOrg.CreateSummary(),
                Email = $"{devicerepoid}-{newuser.Email}",
                PhoneNumber = newuser.PhoneNumber,
                UserName = $"{devicerepoid}-{newuser.Email}",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsAppBuilder = false,
                IsOrgAdmin = false,
                IsUserDevice = true,
                PrimaryDevice = EntityHeader.Create(newuser.Device.Id, newuser.Device.DeviceId),
                DeviceConfiguration = EntityHeader.Create(newuser.Device.DeviceConfiguration.Id, newuser.Device.DeviceConfiguration.Text),
                DeviceRepo = EntityHeader.Create(newuser.Device.DeviceRepository.Id, newuser.Device.DeviceRepository.Text),
                ProfileImageUrl = new ImageDetails()
                {
                    Width = 128,
                    Height = 128,
                    ImageUrl = "https://bytemaster.blob.core.windows.net/userprofileimages/watermark.png",
                    Id = "b78ca749a1e64ce59df4aa100050dcc7"
                }
            };


            SetAuditProperties(appUser);
            SetOwnedProperties(appUser);

            Console.WriteLine("Device Created  - " + newuser.Device.DeviceId);

            try
            {
                var result = await _userManager.CreateAsync(appUser, newuser.Password);
                if (result.Succeeded)
                {
                    var addToOrgResult = await _orgManager.AddUserToOrgAsync(OrgEntityHeader.Id, appUser.Id, OrgEntityHeader, UserEntityHeader);
                    if (addToOrgResult.Successful)
                    {
                        return InvokeResult<AppUser>.Create(appUser);
                    }
                    else
                    {
                        await _userManager.DeleteAsync(appUser);
                        return InvokeResult<AppUser>.FromInvokeResult(addToOrgResult);
                    }
                }
                else
                {
                    Console.WriteLine("Error creating user - removing device - " + newuser.Device.DeviceId);
                    var device = await _deviceManager.GetDeviceByDeviceIdAsync(repo, newuser.Device.DeviceId, OrgEntityHeader, UserEntityHeader);
                    await _deviceManager.DeleteDeviceAsync(repo, device.Id, OrgEntityHeader, UserEntityHeader);
                    return InvokeResult<AppUser>.FromError(result.Errors.First().Description);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Exception - removing device - " + newuser.Device.DeviceId);
                var device = await _deviceManager.GetDeviceByDeviceIdAsync(repo, newuser.Device.DeviceId, OrgEntityHeader, UserEntityHeader);
                await _deviceManager.DeleteDeviceAsync(repo, device.Id, OrgEntityHeader, UserEntityHeader);
                throw;
            }
        }
    }
}
