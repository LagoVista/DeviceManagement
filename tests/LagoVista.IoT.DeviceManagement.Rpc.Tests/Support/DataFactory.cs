using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using Newtonsoft.Json;
using System;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests.Support
{
    public static class DataFactory
    {
        public static Device CreateDevice()
        {
            return new Device()
            {
                Id = Guid.NewGuid().ToId(),
                CreationDate = DateTime.UtcNow.ToJSONString(),
                LastUpdatedDate = DateTime.UtcNow.ToJSONString(),
                CreatedBy = EntityHeader.Create(Guid.NewGuid().ToId(), "abc123"),
                LastUpdatedBy = EntityHeader.Create(Guid.NewGuid().ToId(), "abc123"),
                OwnerOrganization = EntityHeader.Create(Guid.NewGuid().ToId(), "abc123"),
                DeviceId = "dev1234",
                PrimaryAccessKey = "abc123",
                SecondaryAccessKey = "def45",
                Name = "tesedevice",
                DeviceConfiguration = EntityHeader.Create("fff", "ddd"),
                DeviceType = EntityHeader.Create("fff", "ddd"),
            };
        }

        public static DeviceRepository CreateDeviceRespository()
        {
            return JsonConvert.DeserializeObject<DeviceRepository>(Properties.Resources.DeviceRepository);
            //var _user = EntityHeader.Create("3367B1522AF441F39238A85A80B94D33", "Test");
            //var _org = EntityHeader.Create("C8AD4589F26842E7A1AEFBAEFC979C9B", "Test");

            //var repo = new DeviceRepository()
            //{
            //    Id = "04419F4A084A46F0988B2B61D92F0379",
            //    CreatedBy = _user,
            //    CreationDate = DateTime.UtcNow.ToJSONString(),
            //    LastUpdatedBy = _user,
            //    LastUpdatedDate = DateTime.UtcNow.ToJSONString(),
            //    OwnerOrganization = _org,
            //    Name = "MarkTest",
            //    Key = "marktest",
            //    Subscription = EntityHeader.Create("650cf116-0ab9-41d9-817c-1a773e5769b7", "idtext"),
            //    DeviceCapacity = EntityHeader.Create("dev123", "capac"),
            //    StorageCapacity = EntityHeader.Create("storage", "storage"),
            //    RepositoryType = EntityHeader<RepositoryTypes>.Create(RepositoryTypes.NuvIoT)
            //};

            //return repo;
        }
    }
}
