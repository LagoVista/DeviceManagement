// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f63e3dd8fd6ebdab26d40b8cb2179f7008f3ada6a08d08f05c60984a4090b013
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Rpc.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests
{
    [TestClass]
    public class Rpc_IDevicePEMRepo_Tests
    {
        [ClassInitialize]
        static public void ClassInitialize(TestContext testContext)
        {
            DataFactory.Initialize();
        }

        [TestMethod]
        public async Task GetPEMAsync()
        {
            // there's no way to write a pem from here, so the goal is to just make sure neither side crashes with the call
            var pem = await DataFactory.DevicePEMRepo.GetPEMAsync(DataFactory.DeviceRepo, "deviceid", "messageid");
        }

        [TestMethod]
        public async Task GetPEMIndexForDeviceAsync()
        {
            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var response = await DataFactory.DevicePEMRepo.GetPEMIndexForDeviceAsync(DataFactory.DeviceRepo, "deviceid", listRequest);
            Assert.IsTrue(response.Successful);
            //Assert.IsTrue(response.Model.Count() > 0);
        }

        [TestMethod]
        public async Task GetPEMIndexForErrorReasonAsync()
        {
            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25
            };
            var response = await DataFactory.DevicePEMRepo.GetPEMIndexForDeviceAsync(DataFactory.DeviceRepo, "error reason", listRequest);
            Assert.IsTrue(response.Successful);
            //Assert.IsTrue(response.Model.Count() > 0);
        }
    }
}
