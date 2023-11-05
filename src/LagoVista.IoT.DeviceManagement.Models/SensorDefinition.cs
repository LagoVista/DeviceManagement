using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using System;
using System.Collections.Generic;

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

        [EnumLabel(SensorDefinition.IOSensorTypes_ds18b_Idx, DeviceManagementResources.Names.SensorDefinition_Config_IO_DS18B, typeof(DeviceManagementResources))]
        DS18B,

        [EnumLabel(SensorDefinition.IOSensorTypes_dht11_Idx, DeviceManagementResources.Names.SensorDefinition_Config_IO_DHT11, typeof(DeviceManagementResources))]
        DHT11,

        [EnumLabel(SensorDefinition.IOSensorTypes_dht22_Idx, DeviceManagementResources.Names.SensorDefinition_Config_IO_DHT22, typeof(DeviceManagementResources))]
        DHT22,

        [EnumLabel(SensorDefinition.IOSensorTypes_dht22_humidity_Idx, DeviceManagementResources.Names.SensorDefinition_Config_IO_DHT22_Humidity, typeof(DeviceManagementResources))]
        DHT22Humidity,

        [EnumLabel(SensorDefinition.IOSensorTypes_hx711_Idx, DeviceManagementResources.Names.SensorDefinition_Config_IO_HX711, typeof(DeviceManagementResources))]
        HX711,

        [EnumLabel(SensorDefinition.IOSensorTypes_other_Idx, DeviceManagementResources.Names.SensorDefinition_Config_IO_Other, typeof(DeviceManagementResources), SortOrder: 99)]
        Other

    }

    public enum ADCSensorTypes
    {
        [EnumLabel(SensorDefinition.ADCSensorTypes_none_Idx, DeviceManagementResources.Names.SensorDefinition_Config_None, typeof(DeviceManagementResources))]
        None,

        [EnumLabel(SensorDefinition.ADCSensorTypes_adc_Idx, DeviceManagementResources.Names.SensorDefinition_Config_ADC_ADC, typeof(DeviceManagementResources))]
        ADC,

        [EnumLabel(SensorDefinition.ADCSensorTypes_ct_Idx, DeviceManagementResources.Names.SensorDefinition_Config_ADC_CT, typeof(DeviceManagementResources))]
        CurentTransformer,

        [EnumLabel(SensorDefinition.ADCSensorTypes_onoff_Idx, DeviceManagementResources.Names.SensorDefinition_Config_ADC_ONOFF, typeof(DeviceManagementResources))]
        OnOff,

        [EnumLabel(SensorDefinition.ADCSensorTypes_thermistor_Idx, DeviceManagementResources.Names.SensorDefinition_Config_ADC_THERMISTOR, typeof(DeviceManagementResources))]
        Thermistor,

        [EnumLabel(SensorDefinition.ADCSensorTypes_volts_Idx, DeviceManagementResources.Names.SensorDefinition_Config_ADC_Volts, typeof(DeviceManagementResources))]
        Voltage,
   
        [EnumLabel(SensorDefinition.ADCSensorTypes_other_Idx, DeviceManagementResources.Names.SensorDefinition_Config_ADC_Other, typeof(DeviceManagementResources), SortOrder:99)]
        Other
    }

    public enum SensorPorts
    {
        [EnumLabel(SensorDefinition.SensorDefinition_Port1, DeviceManagementResources.Names.SensorDefinition_Ports_Port1, typeof(DeviceManagementResources))]
        Port1,

        [EnumLabel(SensorDefinition.SensorDefinition_Port2, DeviceManagementResources.Names.SensorDefinition_Ports_Port2, typeof(DeviceManagementResources))]
        Port2,

        [EnumLabel(SensorDefinition.SensorDefinition_Port3, DeviceManagementResources.Names.SensorDefinition_Ports_Port3, typeof(DeviceManagementResources))]
        Port3,

        [EnumLabel(SensorDefinition.SensorDefinition_Port4, DeviceManagementResources.Names.SensorDefinition_Ports_Port4, typeof(DeviceManagementResources))]
        Port4,

        [EnumLabel(SensorDefinition.SensorDefinition_Port5, DeviceManagementResources.Names.SensorDefinition_Ports_Port5, typeof(DeviceManagementResources))]
        Port5,

        [EnumLabel(SensorDefinition.SensorDefinition_Port6, DeviceManagementResources.Names.SensorDefinition_Ports_Port6, typeof(DeviceManagementResources))]
        Port6,

        [EnumLabel(SensorDefinition.SensorDefinition_Port7, DeviceManagementResources.Names.SensorDefinition_Ports_Port7, typeof(DeviceManagementResources))]
        Port7,

        [EnumLabel(SensorDefinition.SensorDefinition_Port8, DeviceManagementResources.Names.SensorDefinition_Ports_Port8, typeof(DeviceManagementResources))]
        Port8,
    }


    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.SensorDefinition_Title, DeviceManagementResources.Names.SensorDefinition_Help,
        DeviceManagementResources.Names.SensorDefinition_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources), FactoryUrl: "/api/device/sensordefinition/factory")]
    public class SensorDefinition : IFormDescriptor, IFormConditionalFields
    {
        public const string SensorTechnology_ADC = "adc";
        public const string SensorTechnology_IO = "io";
        public const string SensorTechnology_Bluetooth = "bluetooth";

        public const string SensorValueType_Boolean = "bool";
        public const string SensorValueType_String = "string";
        public const string SensorValueType_Number = "number";

        public const string IOSensorTypes_None = "io_none"; // 0
        public const string IOSensorTypes_input = "io_input"; // 1
        public const string IOSensorTypes_output = "io_output"; // 2
        public const string IOSensorTypes_pulsecounter = "io_pulsecounter"; //3
        public const string IOSensorTypes_ds18b = "io_ds18B"; // 4
        public const string IOSensorTypes_dht11 = "io_dht11"; // 5
        public const string IOSensorTypes_dht22 = "io_dht22"; // 6
        public const string IOSensorTypes_dht22_humidity = "io_dht22_humidity"; // 7
        public const string IOSensorTypes_hx711 = "io_hx711"; // 8
        public const string IOSensorTypes_other = "io_other"; // 99

        public const string ADCSensorTypes_none = "adc_none"; // 0
        public const string ADCSensorTypes_adc = "adc_adc"; // 1
        public const string ADCSensorTypes_ct = "adc_ct"; // 2
        public const string ADCSensorTypes_onoff = "adc_onoff"; // 3
        public const string ADCSensorTypes_thermistor = "adc_thermistor"; // 4
        public const string ADCSensorTypes_volts = "adc_volts"; // 5
        public const string ADCSensorTypes_other = "adc_other"; // 99

        public const string IOSensorTypes_None_Idx = "0";
        public const string IOSensorTypes_input_Idx = "1";
        public const string IOSensorTypes_output_Idx = "2";
        public const string IOSensorTypes_pulsecounter_Idx = "3";
        public const string IOSensorTypes_ds18b_Idx = "4";
        public const string IOSensorTypes_dht11_Idx = "5";
        public const string IOSensorTypes_dht22_Idx = "6";
        public const string IOSensorTypes_dht22_humidity_Idx = "7";
        public const string IOSensorTypes_hx711_Idx = "8";
        public const string IOSensorTypes_other_Idx = "99";

        public const string ADCSensorTypes_none_Idx = "0";
        public const string ADCSensorTypes_adc_Idx = "1";
        public const string ADCSensorTypes_ct_Idx = "2";
        public const string ADCSensorTypes_onoff_Idx = "3";
        public const string ADCSensorTypes_thermistor_Idx = "4";
        public const string ADCSensorTypes_volts_Idx = "5";
        public const string ADCSensorTypes_other_Idx = "99";

        public const string SensorDefinition_Port1 = "1";
        public const string SensorDefinition_Port2 = "2";
        public const string SensorDefinition_Port3 = "3";
        public const string SensorDefinition_Port4 = "4";
        public const string SensorDefinition_Port5 = "5";
        public const string SensorDefinition_Port6 = "6";
        public const string SensorDefinition_Port7 = "7";
        public const string SensorDefinition_Port8 = "8";


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


        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_IconKey, HelpResource: DeviceManagementResources.Names.SensorDefinition_IconKey_Help, WaterMark:DeviceManagementResources.Names.Sensor_SelectIcon,
            FieldType: FieldTypes.Icon, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string IconKey { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_QRCode, HelpResource: DeviceManagementResources.Names.SensorDefinition_QRCode_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string QrCode { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_WebLink, HelpResource: DeviceManagementResources.Names.SensorDefinition_WebLink_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string WebLink { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_UnitsLabel, HelpResource: DeviceManagementResources.Names.SensorDefinition_UnitsLabel_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string UnitsLabel { get; set; }




        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_Units, FieldType: FieldTypes.EntityHeaderPicker, IsUserEditable: true, EnumType: typeof(SensorStates), ResourceType: typeof(DeviceManagementResources), IsRequired: false, WaterMark: DeviceManagementResources.Names.Sensor_Units_Select)]
        public EntityHeader<UnitSet> UnitSet { get; set; }



        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_SensorTypeId, HelpResource: DeviceManagementResources.Names.Sensor_SensorTypeId_Help, FieldType: FieldTypes.Picker, EnumType: (typeof(ADCSensorTypes)), WaterMark: DeviceManagementResources.Names.Sensor_SensorType_Select, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader<ADCSensorTypes> AdcSensorType { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_SensorTypeId, HelpResource: DeviceManagementResources.Names.Sensor_SensorTypeId_Help, FieldType: FieldTypes.Picker, EnumType: (typeof(IOSensorTypes)), WaterMark: DeviceManagementResources.Names.Sensor_SensorType_Select, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader<IOSensorTypes> IoSensorType { get; set; }



        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_Scaler, HelpResource: DeviceManagementResources.Names.SensorDefinition_Scaler_Help, FieldType: FieldTypes.Decimal, IsRequired: false, ResourceType: typeof(DeviceManagementResources))]
        public double? DefaultScaler { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefintiion_Calibration, HelpResource: DeviceManagementResources.Names.SensorDefinition_Calibration_Help, FieldType: FieldTypes.Decimal, IsRequired: false, ResourceType: typeof(DeviceManagementResources))]
        public double? DefaultCalibration { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_Zero, HelpResource: DeviceManagementResources.Names.SensorDefinition_Zero_Help, FieldType: FieldTypes.Decimal, IsRequired: false, ResourceType: typeof(DeviceManagementResources))]
        public double? DefaultZero { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_HasConfigurableThreshold_HighValue, HelpResource: DeviceManagementResources.Names.SensorDefinition_HasConfigurableThreshold_HighValue_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool HasConfigurableThresholdHighValue { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_HasConfigurableThreshold_LowValue, HelpResource: DeviceManagementResources.Names.SensorDefinition_HasConfigurableThreshold_LowValue_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool HasConfigurableThresholdLowValue { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_ServerCalculations, HelpResource: DeviceManagementResources.Names.SensorDefinition_ServerScaling_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool ServerCalculations { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_DefaultLowThreshold, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public double? DefaultLowThreshold { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_DefaultHighThreshold, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public double? DefaultHighThreshold { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_LowThresholdErrorCode, FieldType: FieldTypes.EntityHeaderPicker, EntityHeaderPickerUrl: "/api/errorcodes",
            WaterMark: DeviceManagementResources.Names.Sensor_SelectErroCodeWatermark, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader LowValueErrorCode { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_HighThresholdErrorCode, FieldType: FieldTypes.EntityHeaderPicker, EntityHeaderPickerUrl: "/api/errorcodes",
            WaterMark: DeviceManagementResources.Names.Sensor_SelectErroCodeWatermark, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader HighValueErrorCode { get; set; }



        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_GenerateError_With_On, HelpResource: DeviceManagementResources.Names.SensorDefinition_GenerateError_With_On_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool GenerateErrorWithOn { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_GenerateError_With_Off, HelpResource: DeviceManagementResources.Names.SensorDefinition_GenerateError_With_Off_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool GenerateErrorWithOff { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_OnErrorCode, HelpResource: DeviceManagementResources.Names.SensorDefinition_OnErrorCode_Help, EntityHeaderPickerUrl: "/api/errorcodes",
           WaterMark: DeviceManagementResources.Names.Sensor_SelectErroCodeWatermark, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader OnErrorCode { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_OffErrorCode, HelpResource: DeviceManagementResources.Names.SensorDefinition_OffErrorCode_Help, EntityHeaderPickerUrl: "/api/errorcodes",
           WaterMark:DeviceManagementResources.Names.Sensor_SelectErroCodeWatermark, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader OffErrorCode { get; set; }



        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_DefaultPortIndex, HelpResource: DeviceManagementResources.Names.SensorDefinition_DefaultPortIndex_Help, IsRequired: false, FieldType: FieldTypes.Picker, EnumType: typeof(SensorPorts), WaterMark: DeviceManagementResources.Names.SensorDefinition_DefaultPortIndex_Select, ResourceType: typeof(DeviceManagementResources))]
        public EntityHeader<SensorPorts> DefaultPortIndex { get; set; }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                
                nameof(ValueType),


                nameof(Technology),
                nameof(AdcSensorType),
                nameof(IoSensorType),
                nameof(DefaultPortIndex),

                nameof(UnitsLabel),
                nameof(UnitSet),
                nameof(IconKey),
                nameof(Description),
                nameof(QrCode),
                nameof(WebLink),


                nameof(ServerCalculations),
                nameof(DefaultCalibration),
                nameof(DefaultZero),
                nameof(DefaultScaler),

                nameof(DefaultHighThreshold),
                nameof(DefaultLowThreshold),
                nameof(LowValueErrorCode),
                nameof(HighValueErrorCode),

                nameof(GenerateErrorWithOn),
                nameof(OnErrorCode),
                nameof(GenerateErrorWithOff),
                nameof(OffErrorCode),
            };
        }

        public FormConditionals GetConditionalFields()
        {
            return new FormConditionals()
            {
                ConditionalFields = new List<string>()
                 {
                     nameof(OnErrorCode),
                     nameof(OffErrorCode),
                     nameof(AdcSensorType),
                     nameof(IoSensorType),
                     nameof(DefaultCalibration),
                     nameof(DefaultZero),
                     nameof(DefaultScaler),
                     nameof(DefaultHighThreshold),
                     nameof(DefaultLowThreshold),
                     nameof(LowValueErrorCode),
                     nameof(HighValueErrorCode),
                     nameof(GenerateErrorWithOn),
                     nameof(GenerateErrorWithOff),
                     nameof(OnErrorCode),
                     nameof(OffErrorCode),
                     nameof(ServerCalculations),
                 },
                Conditionals = new List<FormConditional>()
                {
                    new FormConditional()
                    {
                         Field = nameof(Technology),
                         Value = SensorTechnology_ADC,
                         VisibleFields = new List<string>() {nameof(AdcSensorType)},
                         RequiredFields = new List<string>() { nameof(AdcSensorType)},
                    },
                    new FormConditional()
                    {
                         Field = nameof(Technology),
                         Value = SensorTechnology_IO,
                         VisibleFields = new List<string>() {nameof(IoSensorType)},
                         RequiredFields = new List<string>() { nameof(IoSensorType)},
                    },
                    new FormConditional()
                    {
                        Field = nameof(ValueType),
                        Value = SensorValueType_Number,
                        VisibleFields = new List<string>() { nameof(DefaultHighThreshold), nameof(DefaultLowThreshold), nameof(ServerCalculations), 
                            nameof(DefaultScaler), nameof(DefaultCalibration), nameof(DefaultZero), nameof(LowValueErrorCode), nameof(HighValueErrorCode)}
                    },
                    new FormConditional()
                    {
                        Field = nameof(ValueType),
                        Value = SensorValueType_Boolean,
                        VisibleFields = new List<string>() { nameof(GenerateErrorWithOn), nameof(OnErrorCode), nameof(GenerateErrorWithOff), nameof(OffErrorCode)}
                    }
                }
            };
        }
    }
}
