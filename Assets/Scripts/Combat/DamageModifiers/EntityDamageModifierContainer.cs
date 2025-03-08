public sealed class EntityDamageModifierContainer
{
    private DamageModifierContainer _reciverContainer = new();
    private DamageModifierContainer _dealerContainer = new();
    
    public DamageModifierContainer ReciverContainer => _reciverContainer;
    public DamageModifierContainer DealerContainer => _dealerContainer;
}