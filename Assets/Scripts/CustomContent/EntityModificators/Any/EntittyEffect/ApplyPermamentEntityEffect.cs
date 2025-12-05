using System;
using Combat;

public sealed class ApplyPermamentEntityEffect : EntityModificator
{ 
    public override void Enable()
    {
        if (Entity.ComponentsContainer.Has<DraggableObject>())
        {
            Entity.ComponentsContainer.Get<DraggableObject>().Placed += ApplyEffect;
        }
        
        ApplyEffect();
    }

    public override void Disable()
    {
        if (Entity.ComponentsContainer.Has<DraggableObject>())
        {
            Entity.ComponentsContainer.Get<DraggableObject>().Placed -= ApplyEffect;
        }
        
        RemoveEffect();
    }

    private void ApplyEffect()
    {
        Entity.ComponentsContainer.Get<EntityEffectManager>().TryApplyEffect(Type.GetType(Args.GetArgument<string>("EffectName")), Args.GetArgument<int>("Stacks"));
    }

    private void RemoveEffect()
    { 
        Entity.ComponentsContainer.Get<EntityEffectManager>().RemoveEffect(Type.GetType(Args.GetArgument<string>("EffectName")), Args.GetArgument<int>("Stacks"));
    }
}