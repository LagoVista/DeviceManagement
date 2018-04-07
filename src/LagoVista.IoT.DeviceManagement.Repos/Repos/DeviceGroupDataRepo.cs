using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceGroupDataRepo : IDeviceGroupDataRepo
    {
        public DeviceGroupDataRepo(IDeviceManagementSettings repoSettings, IAdminLogger logger)
        {

        }       

        public async Task<ListResponse<DeviceSummaryData>> GetSummaryDataByGroup(DeviceRepository deviceRepo, string groupId)
        {
            //SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var query = @"SELECT c.id, c.Name, c.DeviceId, c.DeviceConfiguration, c.Status, c.DeviceType, c.Attributes, c.Properties,c.GeoLocation, c.Heading, c.Speed, c.LastContact
                         FROM c
                         join dg in c.DeviceGroups
                         where c.EntityType = 'Device'
                          and dg.Id = @deviceGroupId";

            var queryParams = new SqlParameterCollection();
            queryParams.Add(new SqlParameter("@deviceGroupId", groupId));

            //var result = await QueryAsync(query, queryParams);
            //result

            throw new NotImplementedException();
        }
    }
}
