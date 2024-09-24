using Action = Xmlvalidator.Model.Enums.Action;
namespace Xmlvalidator.Model;

public class ActionRule
{
    public int ActionId { get; set; }
    public ConfigField ConfigField { get; set; }
    public Action ActionWhenTrue { get; set; }
    public Action ActionWhenFalse { get; set; }
}