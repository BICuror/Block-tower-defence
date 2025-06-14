using Combat;

public sealed class InvincibleWhileBuidlging : EntityModificator
{
    public override void Enable()
    {
        Entity.Draggable.Placed += AddInvincibility;
        Entity.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted += RemoveInvincibility;
    }

    private void AddInvincibility()
    {
        Entity.ComponentsContainer.Get<EntityEffectManager>().TryApplyEffect(typeof(InvincibilityEffect), 1);
    }
    
    private void RemoveInvincibility()
    {
        Entity.ComponentsContainer.Get<EntityEffectManager>().RemoveEffect(typeof(InvincibilityEffect), 1);
    }

    public override void Disable()
    {
        Entity.Draggable.Placed -= AddInvincibility;
        Entity.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted -= RemoveInvincibility;
        RemoveInvincibility();
    }
}