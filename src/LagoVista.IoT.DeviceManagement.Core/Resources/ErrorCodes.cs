using LagoVista.IoT.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Core.Resources
{
    public class ErrorCodes
    {
        public static ErrorCode DeviceExists = new ErrorCode() { Code = "DM1001", Message = "A Device with that id already exists" };
        public static ErrorCode DeviceExistsInIoTHub = new ErrorCode() { Code = "DM1002", Message = "A Device with that id already exists in your Azure IoT Hub." };
    }
}
