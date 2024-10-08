using Xmlvalidator.Model.Enums;

namespace Xmlvalidator.Model;

public class LogicalRule
{
    public LogicOperator LogicOperator { get; set; }
    public List<ConditionRule>? Conditions { get; set; }
    public List<LogicalRule>? Parents { get; set; }
    public int Order { get; set; }
}