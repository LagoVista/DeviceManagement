﻿using LagoVista.IoT.DeviceManagement.Core.Managers;
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
using LagoVista.IoT.Web.Common.Attributes;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    /// <summary>
    /// Device Group Controller
    /// </summary>
    [Authorize]
    [ConfirmedUser]
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
        public Task<InvokeResult> AddDeviceRepositoryAsync([FromBody] DeviceRepository deviceGroup)
        {
            return _deviceRepositoryManager.AddDeviceRepositoryAsync(deviceGroup, OrgEntityHeader, UserEntityHeader );
        }

        /// <summary>
        /// Device Repositories - Update
        /// </summary>
        /// <param name="deviceGroup"></param>
        /// <returns></returns>
        [HttpPut("/api/devicerepo")]
        public Task<InvokeResult> UpdateDeviceRepositoryAsync([FromBody] DeviceRepository deviceGroup)
        {
            SetUpdatedProperties(deviceGroup);
            return _deviceRepositoryManager.UpdateDeviceRepositoryAsync(deviceGroup, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Repositories - Get for Org
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/devicerepos")]
        public Task<ListResponse<DeviceRepositorySummary>> GetDeviceReposForOrgAsync()
        {
            return _deviceRepositoryManager.GetDeploymentHostsForOrgAsync(OrgEntityHeader.Id, GetListRequestFromHeader(), UserEntityHeader);
        }

        /// <summary>
        /// Device Repositories - Get for Org that are not assinged to an instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/devicerepos/available")]
        public Task<ListResponse<DeviceRepositorySummary>> GetAvailableDeviceReposForOrgAsync()
        {
            return _deviceRepositoryManager.GetAvailableDeploymentHostsForOrgAsync(OrgEntityHeader.Id, GetListRequestFromHeader(), UserEntityHeader);
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
        /// Deployment Repositories - Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/devicerepo/{id}")]
        public async Task<DetailResponse<DeviceRepository>> GetDeviceRepositoryAsync(String id)
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
        public DetailResponse<DeviceRepository> CreateDeviceRep()
        {
            var response = DetailResponse<DeviceRepository>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }

        /// <summary>
        ///  Device Repositories - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/devicerepo/standard/factory")]
        public DetailResponse<DeviceRepository> CreateStandardDeviceRep()
        {
            var response = DetailResponse<DeviceRepository>.Create();
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);
            response.Model.RepositoryType = EntityHeader<RepositoryTypes>.Create(RepositoryTypes.NuvIoT);
            return response;
        }
    }
}

