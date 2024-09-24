// See https://aka.ms/new-console-template for more information

using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Linq;
using Xmlvalidator;
using Xmlvalidator.Model;
using Xmlvalidator.Model.Enums;
using Action = Xmlvalidator.Model.Enums.Action;
using Validator = Xmlvalidator.Validator;

Console.WriteLine("Validator started!");

// Define config fields
var meteringPointCodeConfigField = new ConfigField()
{
    ConfigFieldId = 1,
    NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
    FieldName = "MeteringPointCode"
};
var meteringPointTypeConfigField = new ConfigField()
{
    ConfigFieldId = 1,
    NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
    FieldName = "MeteringPointType"
};
var mpApTypeConfigField = new ConfigField()
{
    ConfigFieldId = 1,
    NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
    FieldName = "MpApType"
};
var meteringPointDataAddressConfigField = new ConfigField()
{
    ConfigFieldId = 1,
    NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
    FieldName = "MeteringPointData_Address"
};
var countryConfigField = new ConfigField()
{
    ConfigFieldId = 1,
    NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
    FieldName = "Country"
};
var isStreetSeparationPresentConfigField = new ConfigField()
{
    ConfigFieldId = 1,
    NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
    FieldName = "IsStreetSeparationPresent"
};
var meteringPointDataAreaConfigField = new ConfigField()
{
    ConfigFieldId = 1,
    NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
    FieldName = "MeteringPointData_Area"
};

// Define actions
var actionForMeteringPointCode = new ActionRule()
{
    ActionId = 1,
    ConfigField = meteringPointCodeConfigField,
    ActionWhenTrue = Action.MUST_EXIST,
    ActionWhenFalse = Action.MUST_NOT_EXIST
};
var actionForIsStreetSeparationPresent = new ActionRule()
{
    ActionId = 2,
    ConfigField = isStreetSeparationPresentConfigField,
    ActionWhenTrue = Action.MUST_EXIST,
    ActionWhenFalse = Action.MUST_NOT_EXIST
};
var actionForMeteringPointDataArea = new ActionRule()
{
    ActionId = 3,
    ConfigField = meteringPointDataAreaConfigField,
    ActionWhenTrue = Action.ALLOW_MULTIPLE,
    ActionWhenFalse = Action.MUST_NOT_EXIST
};

// Define conditions
var conditionForMeteringPointType = new ConditionRule()
{
    ConditionId = 1,
    ConfigField = meteringPointTypeConfigField,
    ConditionOperator = ConditionOperator.IN,
    Value = "CK0314,CK0315,CK0316"
};

var conditionForMeteringCode = new ConditionRule()
{
    ConditionId = 1,
    ConfigField = meteringPointTypeConfigField,
    ConditionOperator = ConditionOperator.EQUALS,
    Value = "590015710563817868"
};

var conditionForMpApType = new ConditionRule()
{
    ConditionId = 1,
    ConfigField = mpApTypeConfigField,
    ConditionOperator = ConditionOperator.HAS_ANY_VALUE,
    Value = null
};

// Define rules

// Simple
var meteringPointLogicalRule = new LogicalRule()
{
    LogicOperator = LogicOperator.AND,
    Conditions = new List<ConditionRule>() { conditionForMeteringPointType },
    Parents = Enumerable.Empty<LogicalRule>().ToList()
};

// Simple
var meteringCodeOrMpApLogicalRule = new LogicalRule()
{
    LogicOperator = LogicOperator.OR,
    Conditions = new List<ConditionRule>() { conditionForMeteringCode, conditionForMpApType },
    Parents = Enumerable.Empty<LogicalRule>().ToList()
};


// Complex Validation
var complexRule = new LogicalRule()
{
    LogicOperator = LogicOperator.OR,
    Conditions = null,
    Parents = new List<LogicalRule>() { meteringPointLogicalRule, meteringCodeOrMpApLogicalRule }
};

var validator = new Validator();
XDocument xmlDoc = XDocument.Load("sample.xml");

var namespaceManager = new XmlNamespaceManager(new NameTable());
namespaceManager.AddNamespace("soap", "http://www.w3.org/2003/05/soap-envelope");
namespaceManager.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
namespaceManager.AddNamespace("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
namespaceManager.AddNamespace("eb", "http://docs.oasis-open.org/ebxml-msg/ebms/v3.0/ns/core/200704/");
namespaceManager.AddNamespace("urn", "urn:cms:b2b:v01");
namespaceManager.AddNamespace("urn1", "urn:pl:oire:unk_2_1_1_1:v1");
namespaceManager.AddNamespace("urn2", "urn:pl:oire:technical:v1");



//validator.ValidateXml(xmlDoc, actionForMeteringPointCode, meteringPointLogicalRule, namespaceManager);
validator.ValidateXml(xmlDoc, actionForMeteringPointCode, complexRule, namespaceManager);
