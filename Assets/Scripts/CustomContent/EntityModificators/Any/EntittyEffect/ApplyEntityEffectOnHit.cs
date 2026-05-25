using System;

public sealed class ApplyEntityEffectOnHit : EntityModificator
{ 
    private ApplyEffectDamageModifier _createdModifier;
    
    public override void Enable()
    {
        _createdModifier = (ApplyEffectDamageModifier)Activator.CreateInstance(typeof(ApplyEffectDamageModifier));
        
        _createdModifier.SetArgumentsContainer(Args);
        
        Entity.ValueModifierContainer.DamageDealerContainer.Add(_createdModifier);
    }

    public override void Disable()
    {
        Entity.ValueModifierContainer.DamageDealerContainer.Remove(_createdModifier);
    }
}