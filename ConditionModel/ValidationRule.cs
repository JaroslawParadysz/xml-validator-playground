namespace Xmlvalidator.ConditionModel;

public class ValidationRule
{
    public int RuleId { get; set; }
    public string RuleName { get; set; }
    public string Operator { get; set; } // AND or OR
    public List<RuleCondition> Conditions { get; set; }
    public List<RuleAction> Actions { get; set; }
}