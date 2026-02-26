using UnityEngine;

public sealed class ScaleDamageWithHeight : EntityModificator
{
    private StatModifier _reachAreaStatModifier;
    private StatModifier _damageStatModifier;
    private float _reachAreaModifierPerStack;
    private float _damageModifierPerStack;
    
    public override void Enable()
    {
        _reachAreaStatModifier = new StatModifier();
        _damageStatModifier = new StatModifier();
        
        _reachAreaModifierPerStack = Args.GetArgument<float>("ReachAreaFlatAddition");
        _damageModifierPerStack = Args.GetArgument<float>("DamageModifier");
        
        Entity.StatContainer.Get<ReachAreaScale>().AddStatModifier(_reachAreaStatModifier);
        Entity.StatContainer.Get<Damage>().AddStatModifier(_damageStatModifier);

        Entity.Draggable.Placed += UpdateScale;
        Entity.Draggable.OnDrag += UpdateScale;
    }

    private void UpdateScale()
    {
        float effectStack = Mathf.Round(Entity.transform.position.y) - 2;
        
        _reachAreaStatModifier.SetFlat(effectStack * _reachAreaModifierPerStack);
        _damageStatModifier.SetMultiplier(effectStack * _damageModifierPerStack);
    }

    public override void Disable()
    {
        Entity.Draggable.Placed -= UpdateScale;
        Entity.Draggable.OnDrag += UpdateScale;
        
        Entity.StatContainer.Get<ReachAreaScale>().RemoveStatModifier(_reachAreaStatModifier);
        Entity.StatContainer.Get<Damage>().RemoveStatModifier(_damageStatModifier);
    }
}