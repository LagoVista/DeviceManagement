using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Repos.Repos.MongoDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LagoVista.Core;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.MongoDBScafolding
{
    [TestClass]
    public class DeviceRepoTests
    {

        [TestMethod]
        public async Task TestIt()
        {
            var repoInfo = new DeviceRepository() { DatabaseName = "nuviot" };

            var id = Guid.NewGuid().ToId();

            var repo = new DeviceManagementRepo();
            var device = new Device();
            device.Id = id;
            device.DeviceId = "KEVIN";
            device.DeviceType = new LagoVista.Core.Models.EntityHeader() { Id = "CASS", Text = "foo" };

            await repo.AddDeviceAsync(repoInfo, device);

            var loadedDevice = await repo.GetDeviceByIdAsync(repoInfo, device.Id);

            Assert.AreEqual(device.DeviceId, loadedDevice.DeviceId);         
        }
    }
}
