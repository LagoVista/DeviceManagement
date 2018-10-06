using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.Rpc.Messages;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Rpc.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests
{
    [TestClass]
    public class Rpc_IDeviceGroupRepo_Tests
    {
        [ClassInitialize]
        static public void ClassInitialize(TestContext testContext)
        {
            DataFactory.Initialize();
        }

        [TestMethod]
        public async Task AddDeviceGroupAsync()
        {
            var groupId = Guid.NewGuid().ToId();
            var createdBy = EntityHeader.Create("userid", "username");
            var deviceGroup = new DeviceGroup
            {
                Id = groupId,
                Name = "name" + groupId,
                IsPublic = false,
                Key = "key" + groupId,
                CreatedBy = createdBy,
                LastUpdatedBy = EntityHeader.Create("userid", "username"),
                OwnerOrganization = EntityHeader.Create("ownerid", "ownername"),
                OwnerUser = EntityHeader.Create("userid", "username"),
                DeviceRepository = EntityHeader.Create(DataFactory.DeviceRepo.Id, DataFactory.DeviceRepo.Name),
                Devices = new List<DeviceGroupEntry>()
                {
                    DeviceGroupEntry.FromDevice( DataFactory.CreateDevice(groupId), createdBy)
                }
            };

            await DataFactory.DeviceGroupRepo.AddDeviceGroupAsync(DataFactory.DeviceRepo, deviceGroup);
        }

        [TestMethod]
        public async Task DeleteDeviceGroupAsync()
        {
            var groupId = Guid.NewGuid().ToId();
            var createdBy = EntityHeader.Create("userid", "username");
            var deviceGroup = new DeviceGroup
            {
                Id = groupId,
                Name = "name" + groupId,
                IsPublic = false,
                Key = "key" + groupId,
                CreatedBy = createdBy,
                LastUpdatedBy = EntityHeader.Create("userid", "username"),
                OwnerOrganization = EntityHeader.Create("ownerid", "ownername"),
                OwnerUser = EntityHeader.Create("userid", "username"),
                DeviceRepository = EntityHeader.Create(DataFactory.DeviceRepo.Id, DataFactory.DeviceRepo.Name),
                Devices = new List<DeviceGroupEntry>()
                {
                    DeviceGroupEntry.FromDevice( DataFactory.CreateDevice(groupId), createdBy)
                }
            };
            await DataFactory.DeviceGroupRepo.AddDeviceGroupAsync(DataFactory.DeviceRepo, deviceGroup);

            await DataFactory.DeviceGroupRepo.DeleteDeviceGroupAsync(DataFactory.DeviceRepo, groupId);
        }

        [TestMethod]
        public async Task GetDeviceGroupAsync()
        {
            var groupId = Guid.NewGuid().ToId();
            var createdBy = EntityHeader.Create("userid", "username");
            var deviceGroup = new DeviceGroup
            {
                Id = groupId,
                Name = "name" + groupId,
                IsPublic = false,
                Key = "key" + groupId,
                CreatedBy = createdBy,
                LastUpdatedBy = EntityHeader.Create("userid", "username"),
                OwnerOrganization = EntityHeader.Create("ownerid", "ownername"),
                OwnerUser = EntityHeader.Create("userid", "username"),
                DeviceRepository = EntityHeader.Create(DataFactory.DeviceRepo.Id, DataFactory.DeviceRepo.Name),
                Devices = new List<DeviceGroupEntry>()
                {
                    DeviceGroupEntry.FromDevice( DataFactory.CreateDevice(groupId), createdBy)
                }
            };
            await DataFactory.DeviceGroupRepo.AddDeviceGroupAsync(DataFactory.DeviceRepo, deviceGroup);

            var response = await DataFactory.DeviceGroupRepo.GetDeviceGroupAsync(DataFactory.DeviceRepo, groupId);
            Assert.AreEqual(groupId, response.Id);
        }

        [TestMethod]
        public async Task GetDeviceGroupsForOrgAsync()
        {
            var groupId = Guid.NewGuid().ToId();
            var createdBy = EntityHeader.Create("userid", "username");
            var orgId = Guid.NewGuid().ToId();
            var deviceGroup = new DeviceGroup
            {
                Id = groupId,
                Name = "name" + groupId,
                IsPublic = false,
                Key = "key" + groupId,
                CreatedBy = createdBy,
                LastUpdatedBy = EntityHeader.Create("userid", "username"),
                OwnerOrganization = EntityHeader.Create(orgId, "ownername"),
                OwnerUser = EntityHeader.Create("userid", "username"),
                DeviceRepository = EntityHeader.Create(DataFactory.DeviceRepo.Id, DataFactory.DeviceRepo.Name),
                Devices = new List<DeviceGroupEntry>()
                {
                    DeviceGroupEntry.FromDevice( DataFactory.CreateDevice(groupId), createdBy)
                }
            };
            await DataFactory.DeviceGroupRepo.AddDeviceGroupAsync(DataFactory.DeviceRepo, deviceGroup);


            var response = await DataFactory.DeviceGroupRepo.GetDeviceGroupsForOrgAsync(DataFactory.DeviceRepo, orgId);
            Assert.IsTrue(response.Count() > 0);
        }

        [TestMethod]
        public async Task QueryKeyInUseAsync(DeviceRepository deviceRepo, string key, string orgId)
        {
            //var deviceId = Guid.NewGuid().ToId();
            //var device = DataFactory.CreateDevice(deviceId);

            //var addDeviceResponse = await DataFactory.DeviceManagementRepoProxy.AddDeviceAsync(DataFactory.DeviceRepo, device);
            //Assert.IsTrue(addDeviceResponse.Successful);

            //var deviceInUse = await DataFactory.DeviceManagementRepoProxy.CheckIfDeviceIdInUse(DataFactory.DeviceRepo, deviceId, DataFactory.OrganizationId);
            //Assert.IsTrue(deviceInUse);

            //deviceId = Guid.NewGuid().ToId();
            //deviceInUse = await DataFactory.DeviceManagementRepoProxy.CheckIfDeviceIdInUse(DataFactory.DeviceRepo, deviceId, DataFactory.OrganizationId);
            //Assert.IsFalse(deviceInUse);
        }

        [TestMethod]
        public Task UpdateDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup)
        {
            throw new NotImplementedException();
        }
    }
}
