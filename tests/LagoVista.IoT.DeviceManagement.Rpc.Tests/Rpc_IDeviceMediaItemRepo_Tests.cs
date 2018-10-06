using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.DeviceManagement.Rpc.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests
{
    [TestClass]
    public class Rpc_IDeviceMediaItemRepo_Tests
    {
        [ClassInitialize]
        static public void ClassInitialize(TestContext testContext)
        {
            DataFactory.Initialize();
        }

        [TestMethod]
        public Task DeleteMediaItemAsync(DeviceRepository repo, string deviceId, string itemId)
        {
            throw new NotImplementedException();
        }

        public Task<DeviceMedia> GetMediaItemAsync(DeviceRepository repo, string deviceId, string itemId)
        {
            throw new NotImplementedException();
        }

        public Task<ListResponse<DeviceMedia>> GetMediaItemsForDeviceAsync(DeviceRepository repo, string deviceId, ListRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<InvokeResult> StoreMediaItemAsync(DeviceRepository repo, DeviceMedia media)
        {
            throw new NotImplementedException();
        }
    }
}
