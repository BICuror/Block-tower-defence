using Configs;
using Combat;

namespace ScriptableLogic.Conditions
{
    public sealed class MoreHealthPercentCondition : Condition, ICondition<CombatEntity>, ICondition
    {
        [Argument("Threshold")] public float _threshold;
        
        public bool GetValue(CombatEntity value)
        {
            return value.Health.GetHpPercent() >= _threshold;
        }
    
        public bool GetValue()
        {
            return GetValue(Args.Owner);
        }
    }
    
    public sealed class LessHealthPercentCondition : Condition, ICondition<CombatEntity>, ICondition
    {
        [Argument("Threshold")] public float _threshold;
        
        public bool GetValue(CombatEntity value)
        {
            return value.Health.GetHpPercent() <= _threshold;
        }
    
        public bool GetValue()
        {
            return GetValue(Args.Owner);
        }
    }
}