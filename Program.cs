// See https://aka.ms/new-console-template for more information

using System.Xml;
using System.Xml.Linq;
using Xmlvalidator;
using Xmlvalidator.ConditionModel;
using Action = System.Action;

Console.WriteLine("Validator started!");

var rules = new List<ValidationRule>
{
    new ValidationRule
    {
        RuleId = 1,
        RuleName = "Rule 1",
        Conditions = new List<RuleCondition>
        {
            new RuleCondition
            {
                ConditionId = 1,
                RuleId = 1,
                NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
                FieldName = "MeteringPointType",
                Operator = Operator.HAS_VALUE
            }
        },
        Actions = new List<RuleAction>
        {
            new RuleAction
            {
                ActionId = 1,
                RuleId = 1,
                NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
                FieldName = "MeteringPointData_Address",
                ActionWhenTrue = Xmlvalidator.ConditionModel.Action.MUST_EXIST,
                ActionWhenFalse = Xmlvalidator.ConditionModel.Action.MUST_NOT_EXIST
            }
        }
    },
    new ValidationRule
    {
        RuleId = 2,
        RuleName = "Rule 2",
        Conditions = new List<RuleCondition>
        {
            new RuleCondition
            {
                ConditionId = 2,
                RuleId = 2,
                NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
                FieldName = "Country",
                Operator = Operator.EQUALS,
                Value = "PL"
                
            }
        },
        Actions = new List<RuleAction>
        {
            new RuleAction
            {
                ActionId = 1,
                RuleId = 1,
                NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
                FieldName = "MeteringPointData_Area",
                ActionWhenTrue = Xmlvalidator.ConditionModel.Action.ALLOW_MULTIPLE,
                ActionWhenFalse = Xmlvalidator.ConditionModel.Action.MUST_NOT_EXIST
            }
        }
    },
    new ValidationRule
    {
        RuleId = 3,
        RuleName = "Rule 3",
        Conditions = new List<RuleCondition>
        {
            new RuleCondition
            {
                ConditionId = 2,
                RuleId = 2,
                NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
                FieldName = "MeteringPointType",
                Operator = Operator.IN,
                Value = "CK0314,CK0315,CK0316"
                
            }
        },
        Actions = new List<RuleAction>
        {
            new RuleAction
            {
                ActionId = 1,
                RuleId = 1,
                NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
                FieldName = "MpApType",
                ActionWhenTrue = Xmlvalidator.ConditionModel.Action.MUST_EXIST,
                ActionWhenFalse = Xmlvalidator.ConditionModel.Action.MUST_NOT_EXIST
            }
        }
    }
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



validator.ValidateXml(xmlDoc, rules, namespaceManager);
