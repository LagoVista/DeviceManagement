using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.DeviceManagement.Models
{
    /// <summary>
    /// THis is a specific sensor on a board.
    /// </summary>
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.Sensor_Title, DeviceManagementResources.Names.Sensor_Help,
        DeviceManagementResources.Names.Sensor_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel,
        typeof(DeviceManagementResources), FactoryUrl: "/api/device/sensor/factory")]
    public class Sensor : IValidateable, IFormDescriptor, IFormDescriptorCol2, IFormConditionalFields
    {
        public Sensor()
        {
            Id = Guid.NewGuid().ToId();
            AlertsEnabled = true;
            State = EntityHeader<SensorStates>.Create(SensorStates.Offline);
            ValueType = EntityHeader<SensorValueType>.Create(SensorValueType.String);
            Value = "";
            Icon = "icon-fo-nerve";
        }

        public const string Sensor_Offline = "offline";
        public const string Sensor_Nominal = "nominal";
        public const string Sensor_Warning = "warning";
        public const string Sensor_Error = "error";
        public const string Sensor_Off = "off";
        public const string Sensor_On = "on";

        public Sensor(SensorDefinition definition) : this()
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            Name = definition.Name;
            Key = definition.Key;
            Icon = definition.Icon;
            ValueType = definition.ValueType;
            Description = definition.Description;
            SensorDefinition = new EntityHeader() { Id = definition.Id, Text = definition.Name, Key = definition.Key };
            Technology = definition.Technology;
            AdcSensorType = definition.AdcSensorType;
            IoSensorType = definition.IoSensorType;
            DeviceScaler = definition.DefaultScaler;
            Zero = definition.DefaultZero;
            Calibration = definition.DefaultCalibration;
            LowThreshold = definition.DefaultLowThreshold;
            HighThreshold = definition.DefaultHighThreshold;
            LowValueErrorCode = definition.LowValueErrorCode;
            HighValueErrorCode = definition.HighValueErrorCode;
            UnitsLabel = definition.UnitsLabel;
            UnitSet = definition.UnitSet;
            OnErrorCode = definition.OnErrorCode;
            OffErrorCode = definition.OffErrorCode;
            GenerateErrorWithOff = definition.GenerateErrorWithOff;
            GenerateErrorWithOn = definition.GenerateErrorWithOn;
        }


        public string Id { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Key, HelpResource: DeviceManagementResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Name { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Description, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string Description { get; set; }

        /// <summary>
        /// BlueTooth Address
        /// </summary>
        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_Address, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string Address { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_AttributeType, FieldType: FieldTypes.Picker, EnumType: typeof(SensorValueType), ResourceType: typeof(DeviceManagementResources), IsRequired: false, WaterMark: DeviceManagementResources.Names.Sensor_AttributeType_Select)]
        public EntityHeader<SensorValueType> ValueType { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_Technology, FieldType: FieldTypes.Picker, EnumType: typeof(SensorTechnology), ResourceType: typeof(DeviceManagementResources), IsRequired: true, WaterMark: DeviceManagementResources.Names.Sensor_Technology_Select)]
        public EntityHeader<SensorTechnology> Technology { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_LastUpdated, FieldType: FieldTypes.ReadonlyLabel, ResourceType: typeof(DeviceManagementResources))]
        public string LastUpdated { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_SensorDefinition, HelpResource: DeviceManagementResources.Names.Sensor_SensorTypeId_Help, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader SensorDefinition { get; set; }

        /// <summary>
        /// Type of Sensor 
        /// </summary>
        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_SensorTypeId, HelpResource: DeviceManagementResources.Names.Sensor_SensorTypeId_Help, FieldType: FieldTypes.Picker, EnumType: (typeof(IOSensorTypes)), WaterMark: DeviceManagementResources.Names.Sensor_SensorType_Select, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader<IOSensorTypes> IoSensorType { get; set; }

        /// <summary>
        /// Type of Sensor 
        /// </summary>
        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_SensorTypeId, HelpResource: DeviceManagementResources.Names.Sensor_SensorTypeId_Help, FieldType: FieldTypes.Picker, EnumType: (typeof(ADCSensorTypes)), WaterMark: DeviceManagementResources.Names.Sensor_SensorType_Select, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader<ADCSensorTypes> AdcSensorType { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_PortIndex, HelpResource: DeviceManagementResources.Names.Sensor_PortIndex_Help, 
            IsRequired: true, FieldType: FieldTypes.Picker, EnumType: typeof(SensorPorts), WaterMark: DeviceManagementResources.Names.SensorDefinition_DefaultPortIndex_Select, ResourceType: typeof(DeviceManagementResources))]
        public EntityHeader<SensorPorts> PortIndexSelection { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_PortIndex,
            HelpResource: DeviceManagementResources.Names.Sensor_PortIndex_Help, IsUserEditable: true, IsRequired: true,
            FieldType: FieldTypes.Integer, ResourceType: typeof(DeviceManagementResources))]
        public int? PortIndex
        {
            get
            {
                if (!EntityHeader.IsNullOrEmpty(PortIndexSelection))
                {
                    switch (PortIndexSelection.Value)
                    {
                        case SensorPorts.Port1: return 0;
                        case SensorPorts.Port2: return 1;
                        case SensorPorts.Port3: return 2;
                        case SensorPorts.Port4: return 3;
                        case SensorPorts.Port5: return 4;
                        case SensorPorts.Port6: return 5;
                        case SensorPorts.Port7: return 6;
                        case SensorPorts.Port8: return 7;
                        default: return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    switch (value.Value)
                    {
                        case 0: PortIndexSelection = EntityHeader<SensorPorts>.Create(SensorPorts.Port1); break;
                        case 1: PortIndexSelection = EntityHeader<SensorPorts>.Create(SensorPorts.Port2); break;
                        case 2: PortIndexSelection = EntityHeader<SensorPorts>.Create(SensorPorts.Port3); break;
                        case 3: PortIndexSelection = EntityHeader<SensorPorts>.Create(SensorPorts.Port4); break;
                        case 4: PortIndexSelection = EntityHeader<SensorPorts>.Create(SensorPorts.Port5); break;
                        case 5: PortIndexSelection = EntityHeader<SensorPorts>.Create(SensorPorts.Port6); break;
                        case 6: PortIndexSelection = EntityHeader<SensorPorts>.Create(SensorPorts.Port7); break;
                        case 7: PortIndexSelection = EntityHeader<SensorPorts>.Create(SensorPorts.Port8); break;
                    }
                }
            }
        }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_DeviceScaler, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public double? DeviceScaler { get; set; }
        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_Calibration, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public double? Calibration { get; set; }
        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_Zero, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public double? Zero { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_LowThreshold, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public double? LowThreshold { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_HighThreshold, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public double? HighThreshold { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_LowThreshold_ErrorCode, FieldType: FieldTypes.EntityHeaderPicker, EntityHeaderPickerUrl: "/api/errorcodes", WaterMark: DeviceManagementResources.Names.Sensor_SelectErroCodeWatermark,
            ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader LowValueErrorCode { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_HighThreshold_ErrorCode, FieldType: FieldTypes.EntityHeaderPicker, EntityHeaderPickerUrl: "/api/errorcodes", WaterMark: DeviceManagementResources.Names.Sensor_SelectErroCodeWatermark,
            ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader HighValueErrorCode { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_AlertsEnabled, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool AlertsEnabled { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_ServerCalculations, HelpResource: DeviceManagementResources.Names.Sensor_ServerScaling_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool ServerCalculations { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_State, FieldType: FieldTypes.Picker, IsUserEditable: false, EnumType: typeof(SensorStates), ResourceType: typeof(DeviceManagementResources), IsRequired: false, WaterMark: DeviceManagementResources.Names.Sensor_State)]
        public EntityHeader<SensorStates> State { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_UnitsLabel, HelpResource: DeviceManagementResources.Names.SensorDefinition_UnitsLabel_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string UnitsLabel { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Common_Icon, HelpResource: DeviceManagementResources.Names.SensorDefinition_UnitsLabel_Help, FieldType: FieldTypes.Icon, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Icon { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_Units, FieldType: FieldTypes.EntityHeaderPicker, IsUserEditable: true, EnumType: typeof(SensorStates), ResourceType: typeof(DeviceManagementResources), IsRequired: false, WaterMark: DeviceManagementResources.Names.Sensor_Units_Select)]
        public EntityHeader<UnitSet> UnitSet { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_OnErrorCode, HelpResource: DeviceManagementResources.Names.SensorDefinition_OnErrorCode_Help, WaterMark: DeviceManagementResources.Names.Sensor_SelectErroCodeWatermark,
            EntityHeaderPickerUrl: "/api/errorcodes", FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader OnErrorCode { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_OffErrorCode, HelpResource: DeviceManagementResources.Names.SensorDefinition_OffErrorCode_Help, WaterMark: DeviceManagementResources.Names.Sensor_SelectErroCodeWatermark,
            EntityHeaderPickerUrl: "/api/errorcodes", FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader OffErrorCode { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_GenerateError_With_On, HelpResource: DeviceManagementResources.Names.SensorDefinition_GenerateError_With_On_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool GenerateErrorWithOn { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_GenerateError_With_Off, HelpResource: DeviceManagementResources.Names.SensorDefinition_GenerateError_With_Off_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool GenerateErrorWithOff { get; set; }


        private string _value;
        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_Value, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false, IsUserEditable: false)]
        public string Value
        {
            get => _value;
            set => _value = value;
        }

        public void PostLoad()
        {
            if (String.IsNullOrEmpty(Name))
                Name = $"{Technology.Text} - {PortIndexSelection.Text}";

            if (String.IsNullOrEmpty(Key))
                Key = Name.Replace(" ", "").Replace("-","").Replace("/","").ToLower();
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(UnitsLabel))
            {
                return Value;
            }
            else
            {
                return $"{Value} {UnitsLabel}";
            }
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
                     nameof(Calibration),
                     nameof(Zero),
                     nameof(DeviceScaler),
                     nameof(HighThreshold),
                     nameof(LowThreshold),
                     nameof(LowValueErrorCode),
                     nameof(HighValueErrorCode),
                     nameof(OnErrorCode),
                     nameof(OffErrorCode),
                     nameof(UnitSet),
                     nameof(ServerCalculations),
                     nameof(AlertsEnabled),
                     nameof(UnitsLabel),
                     nameof(UnitSet)
                 },
                Conditionals = new List<FormConditional>()
                {
                    new FormConditional()
                    {
                         Field = nameof(Technology),
                         Value = Models.SensorDefinition.SensorTechnology_ADC,
                         VisibleFields = new List<string>() {nameof(AdcSensorType)},
                         RequiredFields = new List<string>() {nameof(AdcSensorType)}
                    },
                    new FormConditional()
                    {
                         Field = nameof(Technology),
                         Value =  Models.SensorDefinition.SensorTechnology_IO,
                         VisibleFields = new List<string>() {nameof(IoSensorType)},
                         RequiredFields = new List<string>() {nameof(IoSensorType)}
                         
                    },
                    new FormConditional()
                    {
                        Field = nameof(ValueType),
                        Value =  Models.SensorDefinition.SensorValueType_Number,
                        VisibleFields = new List<string>() { nameof(HighThreshold), nameof(ServerCalculations), nameof(UnitSet), nameof(UnitsLabel),
                            nameof(LowThreshold), nameof(DeviceScaler), nameof(Calibration), nameof(Zero),
                            nameof(LowValueErrorCode), nameof(HighValueErrorCode),nameof(UnitsLabel)},
                        RequiredFields = new List<string>() {nameof(Calibration), nameof(DeviceScaler), nameof(Zero)}

                    },
                    new FormConditional()
                    {
                        Field = nameof(ValueType),
                        Value =  Models.SensorDefinition.SensorValueType_Boolean,
                        VisibleFields = new List<string>() { nameof(OnErrorCode), nameof(OffErrorCode)}
                    }
                }
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(Icon),

                nameof(ValueType),

                nameof(Technology),
                nameof(AdcSensorType),
                nameof(IoSensorType),

                nameof(UnitsLabel),
                nameof(UnitSet),

                nameof(Description),

                nameof(PortIndexSelection),
                
            };
        }

        public List<string> GetFormFieldsCol2()
        {
            return new List<string>() {
                
                nameof(Value),
                nameof(LastUpdated),
                nameof(State),

                nameof(ServerCalculations),
                nameof(Calibration),
                nameof(Zero),                
                nameof(DeviceScaler),

                nameof(AlertsEnabled),

                nameof(HighThreshold),
                nameof(HighValueErrorCode),

                nameof(LowThreshold),
                nameof(LowValueErrorCode),

                nameof(GenerateErrorWithOn),
                nameof(OnErrorCode),
                nameof(GenerateErrorWithOff),
                nameof(OffErrorCode),
            };
        }
    }
}
