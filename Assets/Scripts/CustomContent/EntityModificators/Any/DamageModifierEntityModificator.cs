using System;

public sealed class DamageModifierEntityModificator : EntityModificator
{
    private DamageModifier _damageModifier;
    
    public override void Enable()
    {
        Type modifierType = Type.GetType(Args.GetArgument<string>("DamageModifierTypeName"));
        
        Entity.DamageModifierContainer.DealerContainer.Add(modifierType);
    }

    public override void Disable()
    {
        Type modifierType = Type.GetType(Args.GetArgument<string>("DamageModifierTypeName"));
        
        Entity.DamageModifierContainer.DealerContainer.Remove(modifierType);
    }
}