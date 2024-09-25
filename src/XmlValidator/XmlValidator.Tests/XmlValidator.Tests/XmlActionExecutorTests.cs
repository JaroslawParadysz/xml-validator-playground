using System.Xml;
using System.Xml.Linq;
using Xmlvalidator;
using Xmlvalidator.Model;

using Action = Xmlvalidator.Model.Enums.Action;

namespace XmlValidator.Tests;

public class XmlActionExecutorTests
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
    public void TestValidateXmlWithMustExistAction()
    {
        XDocument xmlDoc = XDocument.Load("SampleFiles/sample.xml");

        var meteringPointTypeConfigField = new ConfigField()
        {
            ConfigFieldId = 1,
            NamespaceURI = "urn:pl:oire:unk_2_1_1_1:v1",
            FieldName = "MeteringPointType"
        };

        var actionRule = new ActionRule()
        {
            ActionWhenTrue = Action.MUST_EXIST,
            ActionWhenFalse = Action.MUST_NOT_EXIST,
            ConfigField = meteringPointTypeConfigField
        };

        bool logicalRuleResult = true;

        var executor = new XmlActionExecutor();
        executor.ApplyAction(xmlDoc, actionRule, logicalRuleResult, _namespaceManager);
        
        Assert.Empty(executor.Errors);

    }
}