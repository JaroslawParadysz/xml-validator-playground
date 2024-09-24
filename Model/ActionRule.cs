namespace Xmlvalidator.ConditionModel;

public class ActionRule
{
    public int ActionId { get; set; }
    public ConfigField ConfigField { get; set; }
    public Action ActionWhenTrue { get; set; }
    public Action ActionWhenFalse { get; set; }
}