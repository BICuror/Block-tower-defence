using Combat;

public sealed class EntityDamageModifierContainer
{
    private DamageModifierContainer _reciverContainer;
    private DamageModifierContainer _dealerContainer;
    
    public DamageModifierContainer ReciverContainer => _reciverContainer;
    public DamageModifierContainer DealerContainer => _dealerContainer;

    public EntityDamageModifierContainer(CombatEntity ownerEntity)
    {
        _reciverContainer = new(ownerEntity);
        _dealerContainer = new(ownerEntity);
    }
}