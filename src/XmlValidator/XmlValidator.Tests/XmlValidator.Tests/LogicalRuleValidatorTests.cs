using System.Xml;
using System.Xml.Linq;
using Xmlvalidator;
using Xmlvalidator.Model;
using Xmlvalidator.Model.Enums;
using Action = Xmlvalidator.Model.Enums.Action;

namespace XmlValidator.Tests
{
    public class LogicalRuleValidatorTests
    {
        private static XmlNamespaceManager _namespaceManager = new XmlNamespaceManagerBuilder()
            .AddNamespace("soap", "http://www.w3.org/2003/05/soap-envelope")
            .AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")
            .AddNamespace("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")
            .AddNamespace("eb", "http://docs.oasis-open.org/ebxml-msg/ebms/v3.0/ns/core/200704/")
            .AddNamespace("urn", "urn:cms:b2b:v01")
            .AddNamespace("urn1", "urn:pl:oire:unk_2_1_1_1:v1")
            .AddNamespace("urn2", "urn:pl:oire:technical:v1")
            .Build();

        [Fact]
        public void TestSimpleLogicalRule()
        {
            XDocument xmlDoc = XDocument.Load("SampleFiles/sample.xml");

            // Define config fields
            var meteringPointTypeConfigField = new ConfigField()
            {
                ConfigFieldId = 1,
                NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
                FieldName = "MeteringPointType"
            };

            // Define conditions
            var conditionForMeteringPointType = new ConditionRule()
            {
                ConditionId = 1,
                ConfigField = meteringPointTypeConfigField,
                ConditionOperator = ConditionOperator.IN,
                Value = "CK0314,CK0315,CK0316"
            };

            // Define logical rules
            var meteringPointLogicalRule = new LogicalRule()
            {
                LogicOperator = LogicOperator.AND,
                Conditions = new List<ConditionRule>() { conditionForMeteringPointType },
                Parents = Enumerable.Empty<LogicalRule>().ToList()
            };

            // Validate XML
            var validator = new LogicalRuleValidator();
            bool result = validator.ValidateLogicalRule(xmlDoc, meteringPointLogicalRule, _namespaceManager);

            // Assert the result
            Assert.True(result, "The XML validation should pass.");
        }
        
        [Fact]
        public void TestComplexLogicalRule()
        {
            XDocument xmlDoc = XDocument.Load("SampleFiles/sample.xml");

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
                ConditionOperator = ConditionOperator.EQUAL,
                Value = "590015710563817868"
            };

            var conditionForMpApType = new ConditionRule()
            {
                ConditionId = 1,
                ConfigField = mpApTypeConfigField,
                ConditionOperator = ConditionOperator.HAS_ANY_VALUE,
                Value = null
            };

            // Define Logical Rules

            // Simple Rule
            var meteringPointLogicalRule = new LogicalRule()
            {
                LogicOperator = LogicOperator.AND,
                Conditions = new List<ConditionRule>() { conditionForMeteringPointType },
                Parents = Enumerable.Empty<LogicalRule>().ToList()
            };

            // Simple Rule
            var meteringCodeOrMpApLogicalRule = new LogicalRule()
            {
                LogicOperator = LogicOperator.OR,
                Conditions = new List<ConditionRule>() { conditionForMeteringCode, conditionForMpApType },
                Parents = Enumerable.Empty<LogicalRule>().ToList()
            };

            // Complex Rule
            var complexRule = new LogicalRule()
            {
                LogicOperator = LogicOperator.OR,
                Conditions = null,
                Parents = new List<LogicalRule>() { meteringPointLogicalRule, meteringCodeOrMpApLogicalRule }
            };

            // Validate XML
            var validator = new LogicalRuleValidator();
            bool result = validator.ValidateLogicalRule(xmlDoc, complexRule, _namespaceManager);

            // Assert the result
            Assert.True(result, "The XML validation should pass.");
        }
    }
}