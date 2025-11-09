// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: cdbb08066e254824eb3421c29e04c239f11733a4fbf8c8e48da66182e3ea5122
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.DeviceManagement.Core.Repos;
using System.Collections.Generic;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Models.UIMetaData;
using System;
using Newtonsoft.Json;
using LagoVista.IoT.DeviceManagement.Core;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceArchiveRepo : LagoVista.CloudStorage.Storage.TableStorageBase<DeviceArchive>, IDeviceArchiveRepo
    {
        IDeviceArchiveReportUtils _deviceArchiveReportUtils;
        
        public DeviceArchiveRepo(IDeviceArchiveReportUtils  deviceArchiveReportUtils, IAdminLogger logger) : base(logger)
        {
            _deviceArchiveReportUtils = deviceArchiveReportUtils;
        }

        public Task AddArchiveAsync(DeviceRepository deviceRepo, DeviceArchive archiveEntry)
        {
            SetTableName(deviceRepo.GetDeviceArchiveStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            return InsertAsync(archiveEntry);
        }

        public Task ClearDeviceArchivesAsync(DeviceRepository deviceRepo, string deviceId)
        {
            return Task.CompletedTask;
        }

        public async Task<ListResponse<List<Object>>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            //TODO: Need to implement filtering
            //TODO: Need to add some bounds here so it won't run forever.
            //return base.GetByFilterAsync(FilterOptions.Create("DateStamp", FilterOptions.Operators.GreaterThan, start), FilterOptions.Create("DateStamp", FilterOptions.Operators.LessThan, end));
            SetTableName(deviceRepo.GetDeviceArchiveStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            var json = await GetRawJSONByParitionIdAsync(deviceId,request.PageSize, request.PageIndex * request.PageSize);
            var rows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

            return _deviceArchiveReportUtils.CreateNormalizedDeviceArchiveResult(rows);
        }
    }
}
