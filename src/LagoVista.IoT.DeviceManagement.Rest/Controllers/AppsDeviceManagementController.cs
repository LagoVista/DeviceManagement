using LagoVista.Core;
using LagoVista.Core.Exceptions;
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
    /// Client API Device Management Services
    /// </summary>
    [Authorize(AuthenticationSchemes = "APIToken")]
    [ConfirmedUser]
    public class AppsDeviceManagementController : ClientAppAPIController
    {
        private IDeviceManager _deviceManager;
        private IOrganizationManager _orgManager;
        private UserManager<AppUser> _userManager;

        public AppsDeviceManagementController(IDeviceRepositoryManager repoManager, IDeviceManager deviceManager,
            IOrganizationManager orgManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(repoManager, userManager, logger)
        {
            _orgManager = orgManager ?? throw new ArgumentNullException(nameof(orgManager));
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <summary>
        /// Client API Device Management - Add New
        /// </summary>
        /// <param name="device"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        [HttpPost("/clientapi/device")]
        public async Task<InvokeResult<Device>> AddDeviceAsync([FromBody] Device device, bool overwrite = false)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.AddDeviceAsync(repo, device, overwrite, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Update
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [HttpPut("/clientapi/device")]
        public async Task<InvokeResult> UpdateDeviceAsync([FromBody] Device device)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            SetUpdatedProperties(device);
            return await _deviceManager.UpdateDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Get devices for a device repository
        /// </summary>
        /// <returns></returns>
        [HttpGet("/clientapi/devices")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForDeviceRepo()
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.GetDevicesForDeviceRepoAsync(repo, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Get For a Location
        /// </summary>
        /// <param name="locationid">Location Id</param>
        /// <returns></returns>
        [HttpGet("/clientapi/location/{locationid}/devices")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForLocationAsync(string locationid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.GetDevicesForLocationIdAsync(repo, locationid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Get Full Devices by Config Id
        /// </summary>
        /// <param name="configid">Configuration Id</param>
        /// <returns></returns>
        [HttpGet("/clientapi/devices/deviceconfig/{configid}/full")]
        public async Task<ListResponse<Device>> GetFullDevicesForConfigAsync(string configid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.GetFullDevicesWithConfigurationAsync(repo, configid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Get Devices by Config Id
        /// </summary>
        /// <param name="configid">Configuration Id</param>
        /// <returns></returns>
        [HttpGet("/clientapi/devices/deviceconfig/{configid}")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForConfigAsync(string configid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.GetDevicesWithConfigurationAsync(repo, configid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Get Devices by Type Id
        /// </summary>
        /// <param name="devicetypeid">Device Type Id</param>
        /// <returns></returns>
        [HttpGet("/clientapi/devices/devicetype/{devicetypeid}")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesForDeviceTypeAsync(string devicetypeid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.GetDevicesWithDeviceTypeAsync(repo, devicetypeid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Search for devices by device id
        /// </summary>
        /// <param name="search">Configuration Id</param>
        /// <returns></returns>
        [HttpGet("/clientapi/devices/search/{search}")]
        public async Task<ListResponse<DeviceSummary>> SearchDeviceAsync(string search)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.SearchByDeviceIdAsync(repo, search, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Get Device Children
        /// </summary>
        /// <param name="parentid">Parent Device Idd</param>
        /// <returns></returns>
        [HttpGet("/clientapi/devices/{parentid}/children")]
        public async Task<ListResponse<DeviceSummary>> GetDeviceChildren(string parentid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.GetChildDevicesAsync(repo, parentid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Get Device Children
        /// </summary>
        /// <param name="parentid">Parent Device Id</param>
        /// <param name="childid">Child Device Id to Attach</param>
        /// <returns></returns>
        [HttpGet("/clientapi/devices/{parentid}/attachchild/{childid}")]
        public async Task<InvokeResult> AttachChildDeviceAsync(string parentid, string childid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.AttachChildDeviceAsync(repo, parentid, childid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Get Device Children
        /// </summary>
        /// <param name="parentid">Parent Device Id</param>
        /// <param name="childid">Child Device Id to be removed</param>
        /// <returns></returns>
        [HttpGet("/clientapi/devices/{parentid}/removechild/{childid}")]
        public async Task<InvokeResult> RemoveChildDeviceAsync(string parentid, string childid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.RemoveChildDeviceAsync(repo, parentid, childid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Get Devices In Status
        /// </summary>
        /// <param name="status">Primary Device Status</param>
        /// <returns></returns>
        [HttpGet("/clientapi/devices/status/{status}")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesInStatusAsync(string status)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.GetDevicesInStatusAsync(repo, status, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// Client API Device Management - Get Devices In Custom Status
        /// </summary>
        /// <param name="customstatus">Custom Status for Device</param>
        /// <returns></returns>
        [HttpGet("/clientapi/devices/customstatus/{customstatus}")]
        public async Task<ListResponse<DeviceSummary>> GetDevicesInCustomStatusConfigAsync(string customstatus)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.GetDevicesInCustomStatusAsync(repo, customstatus, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// Client API Device Management - Get By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/clientapi/device/{id}")]
        public async Task<DetailResponse<Device>> GetDeviceByIdAsync(string id)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            var device = await _deviceManager.GetDeviceByIdAsync(repo, id, OrgEntityHeader, UserEntityHeader);
            if (device == null)
            {
                throw new RecordNotFoundException("Device (by Record Id)", id);
            }
            return DetailResponse<Device>.Create(device.Result);
        }

        /// <summary>
        /// Client API Device Management - Update Status (primary status of device)
        /// </summary>
        /// <param name="id">Unique id of device</param>
        /// <param name="status">Status of device, (not case sensitive) see StatusTypes for device object.</param>
        /// <returns></returns>
        [HttpGet("/clientapi/device/{id}/status/{status}")]
        public async Task<InvokeResult> UpdateDeviceStatusAsync(string id, string status)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.UpdateDeviceStatusAsync(repo, id, status, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Update Status (custom status of device)
        /// </summary>
        /// <param name="id">Unique id of device</param>
        /// <param name="status">Status of device, (not case sensitive) see StatusTypes for device object.</param>
        /// <returns></returns>
        [HttpGet("/clientapi/device/{id}/customstatus/{status}")]
        public async Task<InvokeResult> UpdateDeviceCustomStatusAsync(string id, string status)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.UpdateDeviceCustomStatusAsync(repo, id, status, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Get By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/clientapi/device/{id}/metadata")]
        public async Task<DetailResponse<Device>> GetDeviceByIdAndMetaDataAsync(string id)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            var result = await _deviceManager.GetDeviceByIdAsync(repo, id, OrgEntityHeader, UserEntityHeader, true);
            if (result == null)
            {
                throw new RecordNotFoundException("Device (by Record Id)", id);
            }
            return DetailResponse<Device>.Create(result.Result);
        }

        /// <summary>
        /// Client API Device Management - Get By DeviceId
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("/clientapi/device/deviceid/{deviceid}")]
        public async Task<DetailResponse<Device>> GetDeviceByDeviceId(string deviceid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            var device = await _deviceManager.GetDeviceByDeviceIdAsync(repo, deviceid, OrgEntityHeader, UserEntityHeader);
            if (device == null)
            {
                throw new RecordNotFoundException("Device (by DeviceId)", deviceid);
            }
            return DetailResponse<Device>.Create(device.Result);
        }

        /// <summary>
        /// Client API Device Management - Get By DeviceId
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("/clientapi/device/deviceid/{deviceid}/metadata")]
        public async Task<DetailResponse<Device>> GetDeviceByDeviceIdAndMetaData(string deviceid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            var device = await _deviceManager.GetDeviceByDeviceIdAsync(repo, deviceid, OrgEntityHeader, UserEntityHeader, true);
            if(device == null)
            {
                throw new RecordNotFoundException("Device (by DeviceId)", deviceid);
            }
            return DetailResponse<Device>.Create(device.Result);
        }

        /// <summary>
        /// Client API Device Management - Check Device Id in Use
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("/clientapi/device/inuse/{deviceid}")]
        public async Task<DependentObjectCheckResult> GetDeviceIdInUse(string deviceid)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.CheckIfDeviceIdInUse(repo, deviceid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/clientapi/device/{id}")]
        public async Task<InvokeResult> DeleteDeviceAsync(string id)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.DeleteDeviceAsync(repo, id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Create
        /// </summary>
        /// <returns></returns>
        [HttpGet("/clientapi/device/factory")]
        public async Task<DetailResponse<Device>> CreateDevice()
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();

            var response = DetailResponse<Device>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            /* Note we just create it here for now then the record gets inserted we go ahead assign the name */
            response.Model.DeviceRepository = new EntityHeader() { Id = repo.Id, Text = "TBD" };

            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }

        /// <summary>
        /// Client API Device Management - Create
        /// </summary>
        /// <returns></returns>
        [HttpGet("/clientapi/device/note/factory")]
        public DetailResponse<DeviceNote> CreateDeviceNote()
        {
            var response = DetailResponse<DeviceNote>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);

            return response;
        }

        /// <summary>
        /// Client API Device Management - Add Note to Device
        /// </summary>
        /// <param name="deviceid">Unique id of device</param>
        /// <param name="deviceNote"></param>
        /// <returns></returns>
        [HttpPost("/clientapi/device/{deviceid}/note")]
        public async Task<InvokeResult> AddNoteAsync(string deviceid, [FromBody] DeviceNote deviceNote)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.AddNoteAsync(repo, deviceid, deviceNote, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Add Note to Device
        /// </summary>
        /// <param name="id">Unique id of device</param>
        /// <param name="geolocation"></param>
        /// <returns></returns>
        [HttpPost("/clientapi/device/{deviceid}/geolocation")]
        public async Task<InvokeResult> UpdateGeoLocationAsync(string id, [FromBody] GeoLocation geolocation)
        {
            var repo = await GetDeviceRepositoryWithSecretsAsync();
            return await _deviceManager.UpdateGeoLocationAsync(repo, id, geolocation, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client API Device Management - Add Note to Device
        /// </summary>
        /// <param name="newuser">A new user to be added as a device</param>
        /// <param name="replace"></param>
        /// <returns>App User</returns>
        [OrgAdmin]
        [HttpPost("/clientapi/device/userdevice")]
        public async Task<InvokeResult<AppUser>> AddDeviceUser([FromBody] DeviceUserRegistrationRequest newuser, bool replace = false)
        {
            String userId = Guid.NewGuid().ToId();

            newuser.Device.OwnerUser = EntityHeader.Create(userId, newuser.Email);

            var repo = await GetDeviceRepositoryWithSecretsAsync();

            var addDeviceResult = await _deviceManager.AddDeviceAsync(repo, newuser.Device, replace, OrgEntityHeader, UserEntityHeader);
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
                Email = $"{repo.Id}-{newuser.Email}",
                PhoneNumber = newuser.PhoneNumber,
                UserName = $"{repo.Id}-{newuser.Email}",
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
                    await _deviceManager.DeleteDeviceAsync(repo, device.Result.Id, OrgEntityHeader, UserEntityHeader);
                    return InvokeResult<AppUser>.FromError(result.Errors.First().Description);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Exception - removing device - " + newuser.Device.DeviceId);
                var device = await _deviceManager.GetDeviceByDeviceIdAsync(repo, newuser.Device.DeviceId, OrgEntityHeader, UserEntityHeader);
                await _deviceManager.DeleteDeviceAsync(repo, device.Result.Id, OrgEntityHeader, UserEntityHeader);
                throw;
            }
        }
    }
}
