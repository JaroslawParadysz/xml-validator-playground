using System.Xml;
using System.Xml.Linq;
using Xmlvalidator.ConditionModel;

namespace Xmlvalidator;

public class Validator
{
    public void ValidateXml(XDocument xmlDoc, List<ValidationRule> rules, XmlNamespaceManager namespaceManager)
    {
        foreach (var rule in rules)
        {
            bool conditionResult = EvaluateConditions(xmlDoc, rule.Conditions, namespaceManager);
            ApplyActions(xmlDoc, rule.Actions, conditionResult, namespaceManager);
        }
    }

    private bool EvaluateConditions(XDocument xmlDoc, List<RuleCondition> conditions,
        XmlNamespaceManager namespaceManager)
    {
        foreach (var condition in conditions)
        {
            bool result = EvaluateCondition(xmlDoc, condition, namespaceManager);
            if (!result)
                return false;
        }
        return true;
    }

    private bool EvaluateCondition(XDocument xmlDoc, RuleCondition condition, XmlNamespaceManager namespaceManager)
    {
        IEnumerable<XElement> elements;
        XName elementName = XName.Get(condition.FieldName, condition.NamespaceURI);
        elements = xmlDoc.Descendants(elementName);

        switch (condition.Operators)
        {
            case Operators.HAS_ANY_VALUE:
                return elements.Any(e => !string.IsNullOrEmpty(e.Value));
            case Operators.EQUALS:
                return elements.Any(e => e.Value == condition.Value);
            case Operators.IN:
                var values = condition.Value.Split(',');
                return elements.Any(e => values.Contains(e.Value));
            default:
                throw new InvalidOperationException($"Unsupported operator: {condition.Operators}");
        }
    }

    private void ApplyActions(XDocument xmlDoc, List<RuleAction> actions, bool conditionResult, XmlNamespaceManager namespaceManager)
    {
        foreach (var action in actions)
        {
            ConditionModel.Action actionType = conditionResult ? action.ActionWhenTrue : action.ActionWhenFalse;
            IEnumerable<XElement> elements;
            XName elementName = XName.Get(action.FieldName, action.NamespaceURI);
            elements = xmlDoc.Descendants(elementName);

            switch (actionType)
            {
                case ConditionModel.Action.MUST_EXIST:
                    if (!elements.Any())
                        ReportError($"Field '{action.FieldName}' must exist.");
                    break;
                case ConditionModel.Action.MUST_NOT_EXIST:
                    if (elements.Any())
                        ReportError($"Field '{action.FieldName}' must not exist.");
                    break;
                case ConditionModel.Action.ALLOW_MULTIPLE:
                    break;
                case ConditionModel.Action.DISALLOW_MULTIPLE:
                    if (elements.Count() > 1)
                        ReportError($"Field '{action.FieldName}' must not have multiple instances.");
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported action: {actionType}");
            }
        }
    }

    private void ReportError(string message)
    {
        Console.WriteLine("Validation Error: " + message);
    }

}