using System.Xml;
using System.Xml.Linq;
using Xmlvalidator.Model;
using Action = Xmlvalidator.Model.Enums.Action;

namespace Xmlvalidator;

public class XmlActionExecutor
{
    private List<string> _errors = new List<string>();
    public IReadOnlyCollection<string>  Errors => _errors;
    
    public void ApplyAction(XDocument xmlDoc, ActionRule action, bool conditionResult, XmlNamespaceManager namespaceManager)
    {
        Action actionType = conditionResult ? action.ActionWhenTrue : action.ActionWhenFalse;
        IEnumerable<XElement> elements;
        XName elementName = XName.Get(action.ConfigField.FieldName, action.ConfigField.NamespaceURI);
        elements = xmlDoc.Descendants(elementName);

        switch (actionType)
        {
            case Action.MUST_EXIST:
                if (!elements.Any())
                    ReportError($"Field '{action.ConfigField.FieldName}' must exist.");
                break;
            case Action.MUST_NOT_EXIST:
                if (elements.Any())
                    ReportError($"Field '{action.ConfigField.FieldName}' must not exist.");
                break;
            case Action.ALLOW_MULTIPLE:
                break;
            case Action.DISALLOW_MULTIPLE:
                if (elements.Count() > 1)
                    ReportError($"Field '{action.ConfigField.FieldName}' must not have multiple instances.");
                break;
            default:
                throw new InvalidOperationException($"Unsupported action: {actionType}");
        }
    }
    
    private void ReportError(string message)
    {
        _errors.Add(message);
    }
}