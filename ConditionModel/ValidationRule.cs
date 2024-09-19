namespace Xmlvalidator.ConditionModel;

public class ValidationRule
{
    public int RuleId { get; set; }
    public string RuleName { get; set; }
    public List<RuleCondition> Conditions { get; set; }
    public List<RuleAction> Actions { get; set; }
}