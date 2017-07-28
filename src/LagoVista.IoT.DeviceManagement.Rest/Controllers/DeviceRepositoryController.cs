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
using LagoVista.IoT.ProductStore;
using System.Collections.Generic;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    /// <summary>
    /// Device Group Controller
    /// </summary>
    [Authorize]
    public class DeviceRepositoryController : LagoVistaBaseController
    {

        IDeviceRepositoryManager _deviceRepositoryManager;
        IProductStore _productStore;

        public DeviceRepositoryController(IDeviceRepositoryManager deviceReposistoryManager, IProductStore productStore, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _deviceRepositoryManager = deviceReposistoryManager;
            _productStore = productStore;
        }

        /// <summary>
        /// Device Repositories - Add
        /// </summary>
        /// <param name="deviceGroup"></param>
        /// <returns></returns>
        [HttpPost("/api/devicerepo")]
        public Task<InvokeResult> AddDeviceGroupAsync([FromBody] DeviceRepository deviceGroup)
        {
            return _deviceRepositoryManager.AddDeviceRepositoryAsync(deviceGroup, UserEntityHeader, OrgEntityHeader);
        }

        /// <summary>
        /// Device Repositories - Update
        /// </summary>
        /// <param name="deviceGroup"></param>
        /// <returns></returns>
        [HttpPut("/api/devicerepo")]
        public Task<InvokeResult> UpdateDeviceGroupAsync([FromBody] DeviceRepository deviceGroup)
        {
            SetUpdatedProperties(deviceGroup);
            return _deviceRepositoryManager.UpdateDeviceRepositoryAsync(deviceGroup, OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// Device Repositories - Get for Org
        /// </summary>
        /// <param name="orgId">Organization Id</param>
        /// <returns></returns>
        [HttpGet("/api/org/{orgid}/devicerepos")]
        public async Task<ListResponse<DeviceRepositorySummary>> GetDeviceReposForOrgAsync(String orgId)
        {
            var hostSummaries = await _deviceRepositoryManager.GetDeploymentHostsForOrgAsync(orgId, UserEntityHeader);
            return ListResponse<DeviceRepositorySummary>.Create(hostSummaries);
        }

        /// <summary>
        /// Device Repositories - Can Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/devicerepo/{id}/inuse")]
        public Task<DependentObjectCheckResult> InUseCheck(String id)
        {
            return _deviceRepositoryManager.CheckInUseAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Host - Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/devicerepo/{id}")]
        public async Task<DetailResponse<DeviceRepository>> GetHostAsync(String id)
        {
            var deviceRepo = await _deviceRepositoryManager.GetDeviceRepositoryAsync(id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<DeviceRepository>.Create(deviceRepo);
        }


        /// <summary>
        /// Device Repositories - Key In Use
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/devicegroups/keyinuse/{key}")]
        public Task<bool> QueryKeyInUse(String key)
        {
            return _deviceRepositoryManager.QueryKeyInUserAsync(key, OrgEntityHeader);
        }

        /// <summary>
        /// Device Repositories - Delete
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/api/devicerepo/{id}")]
        public Task<InvokeResult> DeleteDeviceGroupsAsync(string id)
        {
            return _deviceRepositoryManager.DeleteDeviceRepositoryAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        ///  Device Repositories - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/devicerepo/factory")]
        public DetailResponse<DeviceRepository> CreateDeviceGroup()
        {
            var response = DetailResponse<DeviceRepository>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }

        /// <summary>
        /// Device Repository - Get Storage Options.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/api/store/storagecapacity")]
        public async Task<ListResponse<ProductOffering>> GetDeviceStorageSizes()
        {
            var products = await _productStore.GetProductsAsync("storagecapacity");
            return ListResponse<ProductOffering>.Create(products);
        }

        /// <summary>
        /// Device Repository - Get Device Capacity Options.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/api/store/devicecapacity")]
        public async Task<ListResponse<ProductOffering>> GetDeviceCapacityOptions()
        {
            var products = await _productStore.GetProductsAsync("devicecapacity");
            return ListResponse<ProductOffering>.Create(products);
        }
    }
}

