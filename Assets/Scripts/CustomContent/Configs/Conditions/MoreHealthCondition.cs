using Configs;
using Combat;

public sealed class MoreHealthCondition : Condition, ICondition<CombatEntity>, ICondition
{
    [Argument("Threshold")] private float _threshold;
    
    public bool GetValue(CombatEntity value)
    {
        return Args.Owner.Health.GetHp() >= _threshold;
    }

    public bool GetValue()
    {
        return GetValue(Args.Owner);
    }
}

public sealed class LessHealthCondition : Condition, ICondition<CombatEntity>, ICondition
{
    [Argument("Threshold")] private float _threshold;
    
    public bool GetValue(CombatEntity value)
    {
        return Args.Owner.Health.GetHp() <= _threshold;
    }

    public bool GetValue()
    {
        return GetValue(Args.Owner);
    }
}