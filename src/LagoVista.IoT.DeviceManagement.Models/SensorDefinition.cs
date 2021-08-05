using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using System;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public enum SensorTechnology
    {
        [EnumLabel(SensorDefinition.SensorTechnology_ADC, DeviceManagementResources.Names.SensorDefinition_SensorTechnology_ADC, typeof(DeviceManagementResources))]
        ADC,
        
        [EnumLabel(SensorDefinition.SensorTechnology_IO, DeviceManagementResources.Names.SensorDefinition_SensorTechnology_IO, typeof(DeviceManagementResources))]
        IO,

        [EnumLabel(SensorDefinition.SensorTechnology_Bluetooth, DeviceManagementResources.Names.SensorDefinition_SensorTechnology_Bluetooth, typeof(DeviceManagementResources))]
        Bluetooth
    }

    public enum SensorValueType
    {
        [EnumLabel(SensorDefinition.SensorValueType_Boolean, DeviceManagementResources.Names.SensorDefinition_SensorValueType_Boolean, typeof(DeviceManagementResources))]
        Boolean,

        [EnumLabel(SensorDefinition.SensorValueType_String, DeviceManagementResources.Names.SensorDefinition_SensorValueType_String, typeof(DeviceManagementResources))]
        String,

        [EnumLabel(SensorDefinition.SensorValueType_Number, DeviceManagementResources.Names.SensorDefinition_SensorValueType_Number, typeof(DeviceManagementResources))]
        Number,
    }

    public enum IOSensorTypes
    {
        [EnumLabel(SensorDefinition.IOSensorTypes_None_Idx, DeviceManagementResources.Names.SensorDefinition_Config_None, typeof(DeviceManagementResources))]
        None,

        [EnumLabel(SensorDefinition.IOSensorTypes_input_Idx, DeviceManagementResources.Names.SensorDefinition_Config_IO_Input, typeof(DeviceManagementResources))]
        Input,

        [EnumLabel(SensorDefinition.IOSensorTypes_output_Idx, DeviceManagementResources.Names.SensorDefinition_Config_IO_Output, typeof(DeviceManagementResources))]
        Output,

        [EnumLabel(SensorDefinition.IOSensorTypes_pulsecounter_Idx, DeviceManagementResources.Names.SensorDefinition_Config_IO_PulseCounter, typeof(DeviceManagementResources))]
        PulseCounter,

        [EnumLabel(SensorDefinition.IOSensorTypes_ds18b_Idx, DeviceManagementResources.Names.SensorDefinition_SensorValueType_Number, typeof(DeviceManagementResources))]
        DS18B,

        [EnumLabel(SensorDefinition.IOSensorTypes_dht11_Idx, DeviceManagementResources.Names.SensorDefinition_Config_IO_DHT11, typeof(DeviceManagementResources))]
        DHT11,

        [EnumLabel(SensorDefinition.IOSensorTypes_dht22_Idx, DeviceManagementResources.Names.SensorDefinition_Config_IO_DHT22, typeof(DeviceManagementResources))]
        DHT22
    }

    public enum ADCSensorTypes
    {
        [EnumLabel(SensorDefinition.ADCSensorTypes_none_Idx, DeviceManagementResources.Names.SensorDefinition_Config_None, typeof(DeviceManagementResources))]
        None,

        [EnumLabel(SensorDefinition.ADCSensorTypes_adc_Idx, DeviceManagementResources.Names.SensorDefinition_Config_ADC_ADC, typeof(DeviceManagementResources))]
        ADC,

        [EnumLabel(SensorDefinition.ADCSensorTypes_ct_Idx, DeviceManagementResources.Names.SensorDefinition_Config_ADC_CT, typeof(DeviceManagementResources))]
        CurentTransformer,
    }

    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.SensorDefinition_Title, DeviceManagementResources.Names.SensorDefinition_Help,
        DeviceManagementResources.Names.SensorDefinition_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources))]
    public class SensorDefinition
    {
        public const string SensorTechnology_ADC = "adc";
        public const string SensorTechnology_IO = "io";
        public const string SensorTechnology_Bluetooth = "bluetooth";

        public const string SensorValueType_Boolean = "bool";
        public const string SensorValueType_String = "string";
        public const string SensorValueType_Number = "number";

        public const string IOSensorTypes_None = "io_none";
        public const string IOSensorTypes_input = "io_input";
        public const string IOSensorTypes_output = "io_output";
        public const string IOSensorTypes_pulsecounter = "io_pulsecounter";
        public const string IOSensorTypes_ds18b = "io_ds18B";
        public const string IOSensorTypes_dht11 = "io_dht11";
        public const string IOSensorTypes_dht22 = "io_dht22";

        public const string ADCSensorTypes_none = "adc_none";
        public const string ADCSensorTypes_adc = "adc_adc";
        public const string ADCSensorTypes_ct = "adc_ct";

        public const string IOSensorTypes_None_Idx = "0";
        public const string IOSensorTypes_input_Idx = "1";
        public const string IOSensorTypes_output_Idx = "2";
        public const string IOSensorTypes_pulsecounter_Idx = "3";
        public const string IOSensorTypes_ds18b_Idx = "4";
        public const string IOSensorTypes_dht11_Idx = "5";
        public const string IOSensorTypes_dht22_Idx = "6";

        public const string ADCSensorTypes_none_Idx = "0";
        public const string ADCSensorTypes_adc_Idx = "1";
        public const string ADCSensorTypes_ct_Idx = "2";

        public string Id { get; set; }

        public SensorDefinition()
        {
            Id = Guid.NewGuid().ToId();
            ValueType = EntityHeader<SensorValueType>.Create(SensorValueType.Number);
            Technology = null;
        }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_SensorTechnology, HelpResource: DeviceManagementResources.Names.SensorDefinition_SensorTechnology_Help, FieldType: FieldTypes.Picker, EnumType: (typeof(SensorTechnology)), WaterMark: DeviceManagementResources.Names.SensorDefinition_SensorTechnology_Select, ResourceType: typeof(DeviceManagementResources))]
        public EntityHeader<SensorTechnology> Technology { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_SensorValueType, HelpResource: DeviceManagementResources.Names.SensorDefinition_SensorValueType_Help, FieldType: FieldTypes.Picker, EnumType: (typeof(SensorValueType)), WaterMark: DeviceManagementResources.Names.SensorDefinition_SensorValueType_Select, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public EntityHeader<SensorValueType> ValueType { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Common_Key, HelpResource: DeviceManagementResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Name { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Description, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string Description { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_IconKey, HelpResource: DeviceManagementResources.Names.SensorDefinition_IconKey_Help, FieldType: FieldTypes.Text,  ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string IconKey { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_QRCode, HelpResource: DeviceManagementResources.Names.SensorDefinition_QRCode_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string QrCode { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_UnitsLabel, HelpResource: DeviceManagementResources.Names.SensorDefinition_UnitsLabel_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string UnitsLabel { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_WebLink, HelpResource: DeviceManagementResources.Names.SensorDefinition_WebLink_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string WebLink { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_Config, HelpResource: DeviceManagementResources.Names.SensorDefinition_SensorValueType_Help, FieldType: FieldTypes.Picker, EnumType: (typeof(IOSensorTypes)), WaterMark: DeviceManagementResources.Names.SensorDefinition_SensorValueType_Select, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public EntityHeader SensorConfigId { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_HasConfigurableThreshold_HighValue, HelpResource: DeviceManagementResources.Names.SensorDefinition_HasConfigurableThreshold_HighValue_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool HasConfigurableThresholdHighValue { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_HasConfigurableThreshold_LowValue, HelpResource: DeviceManagementResources.Names.SensorDefinition_HasConfigurableThreshold_LowValue_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool HasConfigurableThresholdLowValue { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_DefaultLowThreshold, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public double? DefaultLowThreshold { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_DefaultHighThreshold, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired:false)]
        public double? DefaultHighThreshold { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefintion_LowThresholdErrorCode, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string LowValueErrorCode { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefintion_HighThresholdErrorCode, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string HighValueErrorCode { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefintion_GenerateError_With_On, HelpResource: DeviceManagementResources.Names.SensorDefintion_GenerateError_With_On_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool GenerateErrorWithOn { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefintion_GenerateError_With_Off, HelpResource: DeviceManagementResources.Names.SensorDefintion_GenerateError_With_Off_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool GenerateErrorWithOff { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefintion_OnErrorCode, HelpResource: DeviceManagementResources.Names.SensorDefintion_OnErrorCode_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string OnErrorCode { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefintion_OffErrorCode, HelpResource: DeviceManagementResources.Names.SensorDefintion_OffErrorCode_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string OffErrorCode { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_DefaultSlotIndex, HelpResource: DeviceManagementResources.Names.SensorDefinition_DefaultSlotIndex_Help, FieldType: FieldTypes.Integer, ResourceType: typeof(DeviceManagementResources))]
        public int? DefaultSlotIndex { get; set; }
    }
}
