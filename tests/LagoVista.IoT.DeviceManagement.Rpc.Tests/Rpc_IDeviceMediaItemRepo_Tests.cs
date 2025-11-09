// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1522d52d3981f39415d360c6497912ba90c654b9bd3b1ce2371addbda637878c
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.DeviceManagement.Rpc.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
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
        public async Task DeleteMediaItemAsync()
        {
            var deviceId = Guid.NewGuid().ToId();
            var itemId = Guid.NewGuid().ToId();
            var media = new DeviceMedia
            {
                ItemId = itemId,
                ContentType = "text",
                DeviceId = deviceId,
                FileName = "filename.txt",
                Length = 10,
                TimeStamp = DateTime.Now.ToJSONString(),
                Title = "my file"
            };
            var response = await DataFactory.DeviceMediaItemRepoRemote.StoreMediaItemAsync(DataFactory.DeviceRepo, media);
            Assert.IsTrue(response.Successful);

            await DataFactory.DeviceMediaItemRepo.DeleteMediaItemAsync(DataFactory.DeviceRepo, deviceId, itemId);
        }

        [TestMethod]
        public async Task GetMediaItemAsync()
        {
            var deviceId = Guid.NewGuid().ToId();
            var itemId = Guid.NewGuid().ToId();
            var media = new DeviceMedia
            {
                ItemId = itemId,
                ContentType = "text",
                DeviceId = deviceId,
                FileName = "filename.txt",
                Length = 10,
                TimeStamp = DateTime.Now.ToJSONString(),
                Title = "my file"
            };
            var response = await DataFactory.DeviceMediaItemRepoRemote.StoreMediaItemAsync(DataFactory.DeviceRepo, media);
            Assert.IsTrue(response.Successful);

            var getResponse = await DataFactory.DeviceMediaItemRepo.GetMediaItemAsync(DataFactory.DeviceRepo, deviceId, itemId);
            Assert.AreEqual(itemId, media.ItemId);
        }

        [TestMethod]
        public async Task GetMediaItemsForDeviceAsync()
        {
            var deviceId = Guid.NewGuid().ToId();
            var media = new DeviceMedia
            {
                ItemId = Guid.NewGuid().ToId(),
                ContentType = "text",
                DeviceId = deviceId,
                FileName = "filename.txt",
                Length = 10,
                TimeStamp = DateTime.Now.ToJSONString(),
                Title = "my file"
            };
            var response = await DataFactory.DeviceMediaItemRepoRemote.StoreMediaItemAsync(DataFactory.DeviceRepo, media);
            Assert.IsTrue(response.Successful);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var listResponse = await DataFactory.DeviceMediaItemRepo.GetMediaItemsForDeviceAsync(DataFactory.DeviceRepo, deviceId, listRequest);
            Assert.IsTrue(listResponse.Successful);
            Assert.IsTrue(listResponse.Model.Count() > 0);
        }

        [TestMethod]
        public async Task StoreMediaItemAsync()
        {
            var deviceId = Guid.NewGuid().ToId();
            var media = new DeviceMedia
            {
                ItemId = Guid.NewGuid().ToId(),
                ContentType = "text",
                DeviceId = deviceId,
                FileName = "filename.txt",
                Length = 10,
                TimeStamp = DateTime.Now.ToJSONString(),
                Title = "my file"
            };
            var response = await DataFactory.DeviceMediaItemRepoRemote.StoreMediaItemAsync(DataFactory.DeviceRepo, media);
            Assert.IsTrue(response.Successful);
        }
    }
}
