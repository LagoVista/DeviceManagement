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
using LagoVista.UserAdmin.Interfaces.Managers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
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
        private IDeviceManager _deviceManager;
        private IOrganizationManager _orgManager;
        private IDeviceRepositoryManager _repoManager;
        private UserManager<AppUser> _userManager;

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
        /// <param name="devicerepoid">Device Repoistory Id</param>
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
        /// Device Management - Add Note to Device
        /// </summary>
        /// <param name="devicerepoid">Device Repoistory Id</param>
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
        /// <param name="devicerepoid">Device Repoistory Id</param>
        /// <param name="id">Unique id of device</param>
        /// <param name="errorcode">Error code to clear</param>
        /// <returns></returns>
        [HttpDelete("/api/device/{devicerepoid}/{deviceid}/error/{errorcode}")]
        public async Task<InvokeResult> ClearErrorCodeAsync(string devicerepoid, string deviceid, string errorcode)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.ClearDeviceErrorAsync(repo, deviceid, errorcode, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Management - Add Note to Device
        /// </summary>
        /// <param name="devicerepoid">Device Repoistory Id</param>
        /// <param name="newuser">A new user to be added as a device</param>
        /// <returns>App User</returns>

        [OrgAdmin]
        [HttpPost("/api/device/{devicerepoid}/userdevice")]
        public async Task<InvokeResult<AppUser>> AddDeviceUser(string devicerepoid, [FromBody] DeviceUserRegistrationRequest newuser)
        {
            String userId = Guid.NewGuid().ToId();

            newuser.Device.OwnerUser = EntityHeader.Create(userId, newuser.Email);

            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(newuser.Device.DeviceRepository.Id, OrgEntityHeader, UserEntityHeader);

            var addDeviceResult = await _deviceManager.AddDeviceAsync(repo, newuser.Device, OrgEntityHeader, UserEntityHeader);
            if (!addDeviceResult.Successful)
            {
                return InvokeResult<AppUser>.FromInvokeResult(addDeviceResult);
            }

            var appUser = new AppUser()
            {
                Id = userId,
                FirstName = newuser.FirstName,
                LastName = newuser.LastName,
                CurrentOrganization = OrgEntityHeader,
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
