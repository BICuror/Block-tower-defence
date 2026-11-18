using Configs;

namespace Combat.ValueModifiers
{
    public abstract class DefaultConditionalValueModifier : ValueModifier
    {
        [Argument("FlatChange")] private float _flatChange; 
        [Argument("MultChange")] private float _multChange;

        public override float GetFlatChange(CombatEntity otherEntity, float value)
        {
            if (ConditionIsMet(otherEntity, value)) return _flatChange;

            return 0;
        }
        
        public override float GetMultChange(CombatEntity otherEntity, float value)
        {
            if (ConditionIsMet(otherEntity, value)) return _multChange;

            return 0;
        }
        
        protected abstract bool ConditionIsMet(CombatEntity otherEntity, float value);
    }
}