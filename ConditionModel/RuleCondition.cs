namespace Xmlvalidator.ConditionModel;

public class RuleCondition
{
    public int ConditionId { get; set; }
    public int RuleId { get; set; }
    public string NamespaceURI { get; set; }
    public string FieldName { get; set; }
    public Operators Operators { get; set; }
    public string? Value { get; set; }
}