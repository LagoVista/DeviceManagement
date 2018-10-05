using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Rpc.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
        public Task AddDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup)
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public Task DeleteDeviceGroupAsync(DeviceRepository deviceRepo, string deviceGroupId)
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public Task<DeviceGroup> GetDeviceGroupAsync(DeviceRepository deviceRepo, string id)
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public Task<IEnumerable<DeviceGroupSummary>> GetDeviceGroupsForOrgAsync(DeviceRepository deviceRepo, string orgId)
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public Task<bool> QueryKeyInUseAsync(DeviceRepository deviceRepo, string key, string orgId)
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public Task UpdateDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup)
        {
            throw new NotImplementedException();
        }
    }
}
