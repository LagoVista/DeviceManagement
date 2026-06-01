using LagoVista.Core.Models;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DeviceDataTests
{
    [TestClass]
    public class DeviceSerializationTests
    {

        [TestMethod]
        public void SerializeI()
        {
            var deviceType = new DeviceType()
            {
                WebAppJs = new EntityHeader<string>
                {
                    Id = "fileid",
                    Text = "main.js",
                    Value = "https://example.com/main.js"
                }
            };

            
            var device = new Device
            {
                DeviceType = new EntityHeader<DeviceType>
                {
                    Id = "fileid",
                    Text = "Pool",
                    Value = deviceType
                }
            };

            Console.WriteLine(JsonConvert.SerializeObject(device));
        }
    }
}
