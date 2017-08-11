using LagoVista.IoT.DeviceManagement.Core.Repos;
using System.Collections.Generic;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.PlatformSupport;
using System.Threading.Tasks;
using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceArchiveRepo : LagoVista.CloudStorage.Storage.TableStorageBase<DeviceArchive>, IDeviceArchiveRepo
    {
        public DeviceArchiveRepo(IAdminLogger logger) : base(logger)
        {

        }

        public Task AddArchiveAsync(DeviceRepository deviceRepo, DeviceArchive archiveEntry)
        {
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            return InsertAsync(archiveEntry);
        }

        public async Task<ListResponse<List<String>>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            var resultList = new List<object>();


            var columns = new List<ListColumn>();

            //TODO: Need to implement filtering
            //TODO: Need to add some bounds here so it won't run forever.
            //return base.GetByFilterAsync(FilterOptions.Create("DateStamp", FilterOptions.Operators.GreaterThan, start), FilterOptions.Create("DateStamp", FilterOptions.Operators.LessThan, end));
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            var json = await GetRawJSONByParitionIdAsync(deviceId);
            var rows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

            columns.Add(new ListColumn()
            {
                FieldName = "Timestamp",
                Header = "Timestamp"
            });

            columns.Add(new ListColumn()
            {
                FieldName = "PEMMessageId",
                Header = "PEMMessageId"
            });

            foreach (var row in rows)
            {
                var archive = new List<object>();

                row.Remove(nameof(DeviceArchive.DeviceId));
                row.Remove(nameof(DeviceArchive.DeviceConfigurationVersionId));
                row.Remove(nameof(DeviceArchive.DeviceConfigurationId));
                archive.Add(row[nameof(DeviceArchive.Timestamp)].ToString());
                row.Remove(nameof(DeviceArchive.Timestamp));
                archive.Add(row[nameof(DeviceArchive.PEMMessageId)].ToString());
                row.Remove(nameof(DeviceArchive.PEMMessageId));
                row.Remove("odata.etag");
                row.Remove("RowKey");
                row.Remove("PartitionKey");


                foreach (var key in row.Keys)
                {
                    if (!columns.Where(col => col.FieldName == key).Any())
                    {
                        columns.Add(new ListColumn()
                        {
                            FieldName = key,
                            Header = key,
                        });
                    }

                    archive.Add(row[key]);
                }

                resultList.Add(archive);
            }

            return null;
        }
    }
}
