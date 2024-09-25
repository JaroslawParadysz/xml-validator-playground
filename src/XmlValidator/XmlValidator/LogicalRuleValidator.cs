using System.Xml;
using System.Xml.Linq;
using Xmlvalidator.Model;
using Xmlvalidator.Model.Enums;

namespace Xmlvalidator;

public class LogicalRuleValidator
{
    public bool ValidateLogicalRule(XDocument xmlDoc, LogicalRule logicalRule,
        XmlNamespaceManager namespaceManager)
    {
        if (logicalRule.Parents is null && logicalRule.Conditions is null)
        {
            throw new InvalidOperationException("Logical rule must have at least one parent or condition.");
        }
        
        var parentResult = new List<bool>();
        if (logicalRule.Parents?.Any() ?? false)
        {
            foreach (var parent in logicalRule.Parents)
            {
                parentResult.Add(ValidateLogicalRule(xmlDoc, parent, namespaceManager));
            }
            
            if (logicalRule.LogicOperator == LogicOperator.AND)
            {
                return parentResult.All(x => x);
            }

            if (logicalRule.LogicOperator == LogicOperator.OR)
            {
                return parentResult.Any(x => x);
            }

            throw new InvalidOperationException($"Unsupported operator: {logicalRule.LogicOperator}");
        }

        var conditionsResult = new List<bool>();
        foreach (var condition in logicalRule.Conditions)
        {
            conditionsResult.Add(EvaluateCondition(xmlDoc, condition, namespaceManager));       
        }
        
        if (logicalRule.LogicOperator == LogicOperator.AND)
        {
            return conditionsResult.All(x => x);
        }

        if (logicalRule.LogicOperator == LogicOperator.OR)
        {
            return conditionsResult.Any(x => x);
        }

        throw new InvalidOperationException($"Unsupported operator: {logicalRule.LogicOperator}");
    }

    private bool EvaluateCondition(XDocument xmlDoc, ConditionRule condition, XmlNamespaceManager namespaceManager)
    {
        IEnumerable<XElement> elements;
        XName elementName = XName.Get(condition.ConfigField.FieldName, condition.ConfigField.NamespaceURI);
        elements = xmlDoc.Descendants(elementName);

        switch (condition.ConditionOperator)
        {
            case ConditionOperator.HAS_ANY_VALUE:
                return elements.Any(e => !string.IsNullOrEmpty(e.Value));
            case ConditionOperator.EQUALS:
                return elements.Any(e => e.Value == condition.Value);
            case ConditionOperator.IN:
                var values = condition.Value.Split(',');
                return elements.Any(e => values.Contains(e.Value));
            default:
                throw new InvalidOperationException($"Unsupported operator: {condition.ConditionOperator}");
        }
    }

    private void ReportError(string message)
    {
        Console.WriteLine("Validation Error: " + message);
    }

}