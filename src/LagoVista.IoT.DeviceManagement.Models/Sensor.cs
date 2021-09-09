﻿using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using System;

namespace LagoVista.IoT.DeviceManagement.Models
{
    /// <summary>
    /// THis is a specific sensor on a board.
    /// </summary>
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.Sensor_Title, DeviceManagementResources.Names.Sensor_Help,
        DeviceManagementResources.Names.Sensor_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources))]
    public class Sensor
    {
        public Sensor()
        {
            Id = Guid.NewGuid().ToId();
            AlertsEnabled = true;
            State = EntityHeader<SensorStates>.Create(SensorStates.Offline);
            Value = "";
        }

        public const string Sensor_Offline = "offline";
        public const string Sensor_Nominal = "nominal";
        public const string Sensor_Warning = "warning";
        public const string Sensor_Error = "error";
        public const string Sensor_Off = "off";
        public const string Sensor_On = "on";

        public Sensor(SensorDefinition definition) : this()
        {
            Name = definition.Name;
            Key = definition.Key;
            Description = definition.Description;
            SensorDefinition = new EntityHeader<SensorDefinition>() { Id = definition.Id, Text = definition.Name, Key = definition.Key };
            SensorDefinition.Value = null;
            Technology = definition.Technology;
            SensorType = definition.SensorType;
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
        public EntityHeader<SensorValueType> AttributeType { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_Technology, FieldType: FieldTypes.Picker, EnumType: typeof(SensorTechnology), ResourceType: typeof(DeviceManagementResources), IsRequired: false, WaterMark: DeviceManagementResources.Names.Sensor_Technology_Select)]
        public EntityHeader<SensorTechnology> Technology { get; set; }

        public string LastUpdated { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_SensorDefinition, HelpResource: DeviceManagementResources.Names.Sensor_SensorTypeId_Help, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader<SensorDefinition> SensorDefinition { get; set; }

        /// <summary>
        /// Type of Sensor 
        /// </summary>
        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_SensorTypeId, HelpResource: DeviceManagementResources.Names.Sensor_SensorTypeId_Help, FieldType: FieldTypes.Picker, EnumType: (typeof(IOSensorTypes)), WaterMark: DeviceManagementResources.Names.Sensor_SensorType_Select, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader SensorType { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_PortIndex, HelpResource: DeviceManagementResources.Names.Sensor_PortIndex_Help, IsRequired: false, FieldType: FieldTypes.Picker, EnumType: typeof(SensorPorts), WaterMark: DeviceManagementResources.Names.SensorDefinition_DefaultPortIndex_Select, ResourceType: typeof(DeviceManagementResources))]
        public EntityHeader<SensorPorts> PortIndexSelection { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_PortIndex, HelpResource: DeviceManagementResources.Names.Sensor_PortIndex_Help, IsUserEditable: false, IsRequired: false, FieldType: FieldTypes.Integer, ResourceType: typeof(DeviceManagementResources))]
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
                        default: return null; ;
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
                        case 0: SensorType = EntityHeader<SensorPorts>.Create(SensorPorts.Port1); break;
                        case 1: SensorType = EntityHeader<SensorPorts>.Create(SensorPorts.Port2); break;
                        case 2: SensorType = EntityHeader<SensorPorts>.Create(SensorPorts.Port3); break;
                        case 3: SensorType = EntityHeader<SensorPorts>.Create(SensorPorts.Port4); break;
                        case 4: SensorType = EntityHeader<SensorPorts>.Create(SensorPorts.Port5); break;
                        case 5: SensorType = EntityHeader<SensorPorts>.Create(SensorPorts.Port6); break;
                        case 6: SensorType = EntityHeader<SensorPorts>.Create(SensorPorts.Port7); break;
                        case 7: SensorType = EntityHeader<SensorPorts>.Create(SensorPorts.Port8); break;
                    }
                }
            }
        }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_DeviceScaler, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public double DeviceScaler { get; set; } = 1;
        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_Calibration, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public double Calibration { get; set; } = 1;
        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_Zero, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public double Zero { get; set; } = 0;

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_LowThreshold, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public double? LowThreshold { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_HighThreshold, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public double? HighThreshold { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_LowThreshold_ErrorCode, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string LowValueErrorCode { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_HighThreshold_ErrorCode, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string HighValueErrorCode { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_AlertsEnabled, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool AlertsEnabled { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_State, FieldType: FieldTypes.Picker, IsUserEditable: false, EnumType: typeof(SensorStates), ResourceType: typeof(DeviceManagementResources), IsRequired: false, WaterMark: DeviceManagementResources.Names.Sensor_State)]
        public EntityHeader<SensorStates> State { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_UnitsLabel, HelpResource: DeviceManagementResources.Names.SensorDefinition_UnitsLabel_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string UnitsLabel { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_Units, FieldType: FieldTypes.EntityHeaderPicker, IsUserEditable: true, EnumType: typeof(SensorStates), ResourceType: typeof(DeviceManagementResources), IsRequired: false, WaterMark: DeviceManagementResources.Names.Sensor_Units_Select)]
        public EntityHeader<UnitSet> UnitSet { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_OnErrorCode, HelpResource: DeviceManagementResources.Names.SensorDefinition_OnErrorCode_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string OnErrorCode { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.SensorDefinition_OffErrorCode, HelpResource: DeviceManagementResources.Names.SensorDefinition_OffErrorCode_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public string OffErrorCode { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Sensor_Value, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: false, IsUserEditable: false)]
        public string Value { get; set; }
    }
}