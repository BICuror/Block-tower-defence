using System;

public sealed class ApplyEntityEffectOnHit : EntityModificator
{ 
    private ApplyEffectDamageModifier _createdModifier;
    
    public override void Enable()
    {
        _createdModifier = (ApplyEffectDamageModifier)Activator.CreateInstance(typeof(ApplyEffectDamageModifier));
        
        _createdModifier.SetArgumentsContainer(Args);
        
        Entity.DamageModifierContainer.DealerContainer.Add(_createdModifier);
    }

    public override void Disable()
    {
        Entity.DamageModifierContainer.DealerContainer.Remove(_createdModifier);
    }
}