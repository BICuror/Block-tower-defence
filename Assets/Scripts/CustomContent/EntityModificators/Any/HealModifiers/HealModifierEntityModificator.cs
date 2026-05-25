using System;

public class HealModifierEntityModificator : EntityModificator
{
    private DamageModifier _damageModifier;
    
    public override void Enable()
    {
        Type modifierType = Type.GetType(Args.GetArgument<string>("HealModifierTypeName"));
     
        _damageModifier = (DamageModifier)Activator.CreateInstance(modifierType);
        _damageModifier.SetArgumentsContainer(Args);
        
        bool IsDamageDealerModifier = Args.GetArgument<bool>("IsDealerHealModifier");
        

        if (IsDamageDealerModifier)
        {
            Entity.ValueModifierContainer.HealDealerContainer.Add(_damageModifier);
        }
        else
        {
            Entity.ValueModifierContainer.HealReceiverContainer.Add(_damageModifier);
        }
    }

    public override void Disable()
    {
        bool IsDamageDealerModifier = Args.GetArgument<bool>("IsDealerHealModifier");

        if (IsDamageDealerModifier)
        {
            Entity.ValueModifierContainer.HealDealerContainer.Remove(_damageModifier);
        }
        else
        {
            Entity.ValueModifierContainer.HealReceiverContainer.Remove(_damageModifier);
        }
    }
}