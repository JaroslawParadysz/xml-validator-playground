namespace Xmlvalidator.ConditionModel;

public class RuleAction
{
    public int ActionId { get; set; }
    public string NamespaceURI { get; set; }
    public string FieldName { get; set; }
    public int RuleId { get; set; }
    public Action ActionWhenTrue { get; set; }
    public Action ActionWhenFalse { get; set; }
}