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
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            return InsertAsync(archiveEntry);
        }

        public async Task<ListResponse<List<Object>>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            
            //TODO: Need to implement filtering
            //TODO: Need to add some bounds here so it won't run forever.
            //return base.GetByFilterAsync(FilterOptions.Create("DateStamp", FilterOptions.Operators.GreaterThan, start), FilterOptions.Create("DateStamp", FilterOptions.Operators.LessThan, end));
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            var json = await GetRawJSONByParitionIdAsync(deviceId);
            var rows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

            return _deviceArchiveReportUtils.CreateNormalizedDeviceArchiveResult(rows);
        }
    }
}
