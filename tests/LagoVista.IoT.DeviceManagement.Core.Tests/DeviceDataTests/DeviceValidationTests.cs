using LagoVista.Core.Validation;
using LagoVista.Core;
using LagoVista.IoT.DeviceManagement.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core.Models;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DeviceDataTests
{
    [TestClass]
    public class DeviceValidationTests
    {
        private void WriteErrors(ValidationResult result)
        {
            foreach (var err in result.Errors)
            {
                Console.WriteLine(err.Message);
            }
        }

        private Device CreateValidDevice()
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
                Name = "devicename",
                DeviceConfiguration = EntityHeader.Create("fff", "ddd"),
                DeviceType = EntityHeader.Create("fff", "ddd"),
            };
        }

        [TestMethod]
        public void Device_Validation_CustomField_Validates_Invalid()
        {
            var device = CreateValidDevice();
            device.PropertiesMetaData = new List<DeviceAdmin.Models.CustomField>()
            {
                new DeviceAdmin.Models.CustomField()
                {
                    FieldType = EntityHeader<DeviceAdmin.Models.ParameterTypes>.Create(DeviceAdmin.Models.ParameterTypes.String),
                    IsRequired = true,
                    Label = "Required Field One",
                    Key = "fieldkey"
                }
            };

            var result = Validator.Validate(device);
            WriteErrors(result);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("Required Field One is a required field.", result.Errors.First()?.Message);
        }

        [TestMethod]
        public void Device_Validation_CustomField_Validates_Valid()
        {
            var device = CreateValidDevice();
            device.PropertiesMetaData = new List<DeviceAdmin.Models.CustomField>()
            {
                new DeviceAdmin.Models.CustomField()
                {
                    FieldType = EntityHeader<DeviceAdmin.Models.ParameterTypes>.Create(DeviceAdmin.Models.ParameterTypes.String),
                    IsRequired = true,
                    Label = "Required Field One",
                    Key = "fieldkey"
                }
            };

            device.Properties.Add(new AttributeValue()
            {
                Value = "1234"
            });

            var result = Validator.Validate(device);
            WriteErrors(result);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("Required Field One is a required field.", result.Errors.First()?.Message);
        }
    }
}
