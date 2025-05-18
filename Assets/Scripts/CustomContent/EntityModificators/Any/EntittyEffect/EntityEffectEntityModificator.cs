using System;

public sealed class EntityEffectEntityModificator : EntityModificator
{ 
    private ApplyEffectDamageModifier _createdModifier;
    
    public override void Enable()
    {
        _createdModifier = (ApplyEffectDamageModifier)Activator.CreateInstance(typeof(ApplyEffectDamageModifier));
        
        _createdModifier.SetEffectData(Type.GetType(Args.GetArgument<string>("EffectTypeName")), Args.GetArgument<int>("EffectStrength"), Args.GetArgument<float>("EffectDuration"));
        
        Entity.DamageModifierContainer.DealerContainer.Add(_createdModifier);
    }

    public override void Disable()
    {
        Entity.DamageModifierContainer.DealerContainer.Remove(_createdModifier);
    }
}