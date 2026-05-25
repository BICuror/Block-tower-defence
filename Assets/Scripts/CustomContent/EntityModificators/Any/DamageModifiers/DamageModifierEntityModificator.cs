using System;

public sealed class DamageModifierEntityModificator : EntityModificator
{
    private DamageModifier _damageModifier;
    
    public override void Enable()
    {
        Type modifierType = Type.GetType(Args.GetArgument<string>("DamageModifierTypeName"));
     
        _damageModifier = (DamageModifier)Activator.CreateInstance(modifierType);
        _damageModifier.SetArgumentsContainer(Args);
        
        bool IsDamageDealerModifier = Args.GetArgument<bool>("IsDealerDamageModifier");
        

        if (IsDamageDealerModifier)
        {
            Entity.ValueModifierContainer.DamageDealerContainer.Add(_damageModifier);
        }
        else
        {
            Entity.ValueModifierContainer.DamageReceiverContainer.Add(_damageModifier);
        }
    }

    public override void Disable()
    {
        bool IsDamageDealerModifier = Args.GetArgument<bool>("IsDealerDamageModifier");

        if (IsDamageDealerModifier)
        {
            Entity.ValueModifierContainer.DamageDealerContainer.Remove(_damageModifier);
        }
        else
        {
            Entity.ValueModifierContainer.DamageReceiverContainer.Remove(_damageModifier);
        }
    }
}