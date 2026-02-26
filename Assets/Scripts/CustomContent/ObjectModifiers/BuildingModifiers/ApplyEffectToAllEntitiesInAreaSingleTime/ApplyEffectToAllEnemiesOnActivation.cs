using Cashing;
using Combat;

public sealed class ApplyEffectToAllEnemiesOnActivation : ApplyEffectOnceToEntitiesInArea
{
    [Cached] private CombatEntity _ownerEntity;
    
    private void Start()
    {
        base.Start();
        _ownerEntity.Activated += ApplyEffect;
    }
    
    private void OnDestroy()
    {
        _ownerEntity.Activated -= ApplyEffect;
    }
}