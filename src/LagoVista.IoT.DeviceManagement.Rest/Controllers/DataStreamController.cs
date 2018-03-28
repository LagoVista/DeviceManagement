using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    /// <summary>
    /// Device Stream Controller
    /// </summary>
    [Authorize]
    public class DataStreamController : LagoVistaBaseController
    {
        IDataStreamManager _dataStreamManager;

        public DataStreamController(IDataStreamManager dataStreamManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _dataStreamManager = dataStreamManager;
        }

        /// <summary>
        /// Data Stream - Add
        /// </summary>
        /// <param name="datastream"></param>
        /// <returns></returns>
        [HttpPost("/api/datastream")]
        public Task<InvokeResult> AddHostAsync([FromBody] DataStream datastream)
        {
            return _dataStreamManager.AddDataStreamAsync(datastream, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Data Stream - Get
        /// </summary>
        /// <returns>A Data Stream</returns>
        [HttpGet("/api/datastream/{id}")]
        public async Task<DetailResponse<DataStream>> DataDataStreamAsync(string id)
        {
            var dataStream = await _dataStreamManager.GetDataStreamAsync(id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<DataStream>.Create(dataStream);
        }

        /// <summary>
        /// Data Stream - Update
        /// </summary>
        /// <param name="datastream"></param>
        /// <returns></returns>
        [HttpPut("/api/datastream")]
        public Task<InvokeResult> UpdateDataStreamAsync([FromBody] DataStream datastream)
        {
            SetUpdatedProperties(datastream);
            return _dataStreamManager.UpdateDataStreamAsync(datastream, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Data Stream - Get for Current Org
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/datastreams")]
        public async Task<ListResponse<DataStreamSummary>> GetDataStreamsForOrgAsync()
        {
            var hostSummaries = await _dataStreamManager.GetDataStreamsForOrgAsync(OrgEntityHeader.Id, UserEntityHeader);
            var response = ListResponse<DataStreamSummary>.Create(hostSummaries);

            return response;
        }

        /// <summary>
        /// Data Stream - Can Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/datastream/{id}/inuse")]
        public Task<DependentObjectCheckResult> InUseCheck(String id)
        {
            return _dataStreamManager.CheckDataStreamInUseAsync(id, OrgEntityHeader, UserEntityHeader);
        }
        
        /// <summary>
        /// Data Stream - Key In Use
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/datastream/{key}/keyinuse")]
        public Task<bool> HostKeyInUse(String key)
        {
            return _dataStreamManager.QueryKeyInUseAsync(key, OrgEntityHeader);
        }

        /// <summary>
        /// Data Stream - Delete
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/api/datastream/{id}")]
        public Task<InvokeResult> DeleteDataStreamAsync(string id)
        {
            return _dataStreamManager.DeleteDatStreamAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Data Stream - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/datastream/factory")]
        public DetailResponse<DataStream> CreateDataStream()
        {

            var response = DetailResponse<DataStream>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }

        /// <summary>
        /// Data Stream - Create New Field
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/datastreamfield/factory")]
        public DetailResponse<DataStreamField> CreateDataStreamField()
        {
            return DetailResponse<DataStreamField>.Create();
        }        
    }
}
