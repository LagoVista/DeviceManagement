using LagoVista.IoT.Logging;

namespace LagoVista.IoT.DeviceManagement.Core.Resources
{
    public class ErrorCodes
    {
        public static ErrorCode DeviceExists = new ErrorCode() { Code = "DM1001", Message = "A Device with that id already exists" };
        public static ErrorCode DeviceExistsInIoTHub = new ErrorCode() { Code = "DM1002", Message = "A Device with that id already exists in your Azure IoT Hub." };
        public static ErrorCode PEMDoesNotExist = new ErrorCode() { Code = "DM1003", Message = "Could not find PEM" };
        public static ErrorCode SetStatus_InvalidOption = new ErrorCode() { Code = "DM1004", Message = "Invalid state option" };
        public static ErrorCode CouldNotFindDeviceWithId = new ErrorCode() { Code = "DM1005", Message = "Could not find device with that unique id" };
        public static ErrorCode CouldNotFindDeviceWithDeviceId = new ErrorCode() { Code = "DM1006", Message = "Could not find device with that device id" };
    }
}
