using System.Xml;
using System.Xml.Linq;
using Xmlvalidator.ConditionModel;

namespace Xmlvalidator;

public class Validator
{
    public void ValidateXml(XDocument xmlDoc, ActionRule action, LogicalRule validationRule, XmlNamespaceManager namespaceManager)
    {
        var validationResult = ValidateCondition(xmlDoc, action, validationRule, namespaceManager);
        
        ApplyActions(xmlDoc, action, validationResult, namespaceManager);
    }

    public bool ValidateCondition(XDocument xmlDoc, ActionRule action, LogicalRule logicalRule,
        XmlNamespaceManager namespaceManager)
    {
        var parentResult = new List<bool>();
        if (logicalRule.Parents.Any())
        {
            foreach (var parent in logicalRule.Parents)
            {
                parentResult.Add(ValidateCondition(xmlDoc, action, parent, namespaceManager));
            }
            
            if (logicalRule.LogicOperator == LogicOperator.AND)
            {
                return parentResult.All(x => x);
            }
            else if (logicalRule.LogicOperator == LogicOperator.OR)
            {
                return parentResult.Any(x => x);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported operator: {logicalRule.LogicOperator}");
            }
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
        else if (logicalRule.LogicOperator == LogicOperator.OR)
        {
            return conditionsResult.Any(x => x);
        }
        else
        {
            throw new InvalidOperationException($"Unsupported operator: {logicalRule.LogicOperator}");
        }
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

    private void ApplyActions(XDocument xmlDoc, ActionRule action, bool conditionResult, XmlNamespaceManager namespaceManager)
    {
        ConditionModel.Action actionType = conditionResult ? action.ActionWhenTrue : action.ActionWhenFalse;
        IEnumerable<XElement> elements;
        XName elementName = XName.Get(action.ConfigField.FieldName, action.ConfigField.NamespaceURI);
        elements = xmlDoc.Descendants(elementName);

        switch (actionType)
        {
            case ConditionModel.Action.MUST_EXIST:
                if (!elements.Any())
                    ReportError($"Field '{action.ConfigField.FieldName}' must exist.");
                break;
            case ConditionModel.Action.MUST_NOT_EXIST:
                if (elements.Any())
                    ReportError($"Field '{action.ConfigField.FieldName}' must not exist.");
                break;
            case ConditionModel.Action.ALLOW_MULTIPLE:
                break;
            case ConditionModel.Action.DISALLOW_MULTIPLE:
                if (elements.Count() > 1)
                    ReportError($"Field '{action.ConfigField.FieldName}' must not have multiple instances.");
                break;
            default:
                throw new InvalidOperationException($"Unsupported action: {actionType}");
        }
    }

    private void ReportError(string message)
    {
        Console.WriteLine("Validation Error: " + message);
    }

}