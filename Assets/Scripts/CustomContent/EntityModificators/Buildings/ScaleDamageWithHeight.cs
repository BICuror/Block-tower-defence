using UnityEngine;

public sealed class ScaleDamageWithHeight : EntityModificator
{
    private StatModifier _statModifier;
    
    public override void Enable()
    {
        _statModifier = new StatModifier();
        
        Entity.StatContainer.Get<Damage>().AddStatModifier(_statModifier);

        Entity.Draggable.Placed += UpdateScale;
    }

    private void UpdateScale()
    {
        _statModifier.SetMultiplier((Mathf.Round(Entity.transform.position.y) - 2) * 0.25f);
    }

    public override void Disable()
    {
        Entity.Draggable.Placed -= UpdateScale;
        
        Entity.StatContainer.Get<Damage>().RemoveStatModifier(_statModifier);
    }
}