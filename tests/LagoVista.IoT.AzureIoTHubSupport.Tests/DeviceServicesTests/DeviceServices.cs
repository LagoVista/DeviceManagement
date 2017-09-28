using LagoVista.IoT.AzureIoTHubSupport.Services;
using LagoVista.IoT.AzureIoTHubSupport.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.AzureIoTHubSupport.Tests.DeviceServicesTests
{
    [TestClass]
    public class DeviceServicesTests
    {
        [TestMethod]

        public async Task GetDevicesAsyncTests()
        {
            var srvcs = new DeviceServices("iothubowner", "1voUqXL1XNfWbjjPutl9159kP9eKDLu95VCFOSSGCAY=", "bytemaster");
            var devices = await srvcs.GetDevicesAsync();
        }

        [TestMethod]
        public async Task GetDeviceAsyncTest()
        {
            var srvcs = new DeviceServices("iothubowner", "1voUqXL1XNfWbjjPutl9159kP9eKDLu95VCFOSSGCAY=", "bytemaster");
            var device = await srvcs.GetDeviceAsync("device001");
            Assert.AreEqual("device001",device.DeviceId);
        }

        [TestMethod]
        public async Task UpdateDeviceAsyncTest()
        {
            var srvcs = new DeviceServices("iothubowner", "1voUqXL1XNfWbjjPutl9159kP9eKDLu95VCFOSSGCAY=", "bytemaster");
            var device = await srvcs.GetDeviceAsync("device001");
            await srvcs.UpdateDeviceAsync(device);

            Assert.AreEqual("device001", device.DeviceId);
        }

        [TestMethod]
        public async Task CreateDeviceTest()
        {
            var srvcs = new DeviceServices("iothubowner", "1voUqXL1XNfWbjjPutl9159kP9eKDLu95VCFOSSGCAY=", "bytemaster");
           
            var device = await srvcs.AddDeviceAsync("device004");
            Assert.IsNotNull(device);
            Assert.AreEqual("device004", device.DeviceId);
        }
    }
}