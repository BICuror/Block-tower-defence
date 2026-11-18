using Configs;

namespace Combat.ValueModifiers
{
    public abstract class ValueModifier : ArgumentsRequester
    {
        protected CombatEntity OwnerEntity;
        
        public virtual ValueModifierOrder Order => ValueModifierOrder.DefaultDamageCalculation;

        public void Initialize(CombatEntity ownerEntity)
        {
            OwnerEntity = ownerEntity;
            
            OnInitialize();
        }
        
        protected virtual void OnInitialize() {}
    
        public virtual float GetFlatChange(CombatEntity otherEntity, float value) => 0;
        public virtual float GetMultChange(CombatEntity otherEntity, float value) => 0;
        public virtual bool AllowValuePass(CombatEntity otherEntity, float value) => true;
    }
    
    public enum ValueModifierOrder
    {
        PreDamageCalculation = 0,
        EarlyDamageCalculation = 1,
        DefaultDamageCalculation = 2,
        PostDamageCalculation = 3
    }
}