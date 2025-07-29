using System;
using Combat;

public sealed class ApplyModifierUntilPickUpEntityModifier : EntityModificator
{
    public override bool CanBeApplied() => !Entity.ComponentsContainer.Get<EntityModificatorsContainer>().Has(Args.GetArgument<EntityModificatorData>("EntityModificatorData"));
    
    public override void Enable()
    {
        Entity.Draggable.PickedUp += Remove;

        Type effectType = Type.GetType(Args.GetArgument<string>("EffectTypeName"));
        
        Entity.ComponentsContainer.Get<EntityEffectManager>().TryApplyEffect(effectType, Args.GetArgument<int>("EffectStrength"));
    }

    public override void Disable()
    {
        Entity.Draggable.PickedUp -= Remove;
        
        Type effectType = Type.GetType(Args.GetArgument<string>("EffectTypeName"));
        
        Entity.ComponentsContainer.Get<EntityEffectManager>().RemoveEffect(effectType, Args.GetArgument<int>("EffectStrength"));
    }

    private void Remove()
    {
        Entity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(Args.GetArgument<EntityModificatorData>("EntityModificatorData"));
    }
}