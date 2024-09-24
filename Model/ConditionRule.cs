namespace Xmlvalidator.ConditionModel;

public class ConditionRule
{
    public int ConditionId { get; set; }
    public ConfigField ConfigField { get; set; }
    public ConditionOperator ConditionOperator { get; set; }
    public string? Value { get; set; }
}