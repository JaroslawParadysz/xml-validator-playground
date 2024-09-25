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
        var anyParents = logicalRule.Parents?.Any() ?? false;
        var anyConditions = logicalRule.Conditions?.Any() ?? false;
        if (!anyParents && !anyConditions)
        {
            throw new InvalidOperationException("Logical rule must have at least one parent or condition.");
        }
        
        if (anyParents && anyConditions)
        {
            throw new InvalidOperationException("Logical rule can not have both parent and condition.");
        }
        
        if (anyParents)
        {
            return ValidateParents(xmlDoc, logicalRule, namespaceManager);
        }

        return ValidateConditions(xmlDoc, logicalRule, namespaceManager);
    }

    private bool ValidateConditions(XDocument xmlDoc, LogicalRule logicalRule, XmlNamespaceManager namespaceManager)
    {
        if (logicalRule.Conditions is null || !(logicalRule.Conditions?.Any() ?? false))
        {
            throw new InvalidOperationException("Conditions can not be null.");
        }
        
        var result = new List<bool>();
        foreach (var condition in logicalRule.Conditions)
        {
            result.Add(EvaluateCondition(xmlDoc, condition, namespaceManager));       
        }
        
        return logicalRule.LogicOperator switch
        {
            LogicOperator.AND => result.All(x => x),
            LogicOperator.OR => result.Any(x => x),
            _ => throw new InvalidOperationException($"Unsupported operator: {logicalRule.LogicOperator}")
        };
    }

    private bool ValidateParents(XDocument xmlDoc, LogicalRule logicalRule, XmlNamespaceManager namespaceManager)
    {
        if (logicalRule.Parents is null || !(logicalRule.Parents?.Any() ?? false))
        {
            throw new InvalidOperationException("Parents can not be null.");
        }

        var result = new List<bool>();
        foreach (var parent in logicalRule.Parents)
        {
            result.Add(ValidateLogicalRule(xmlDoc, parent, namespaceManager));
        }
            
        return logicalRule.LogicOperator switch
        {
            LogicOperator.AND => result.All(x => x),
            LogicOperator.OR => result.Any(x => x),
            _ => throw new InvalidOperationException($"Unsupported operator: {logicalRule.LogicOperator}")
        };
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