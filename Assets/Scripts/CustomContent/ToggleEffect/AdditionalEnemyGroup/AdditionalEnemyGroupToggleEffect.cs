using Zenject;

public sealed class AdditionalEnemyGroupToggleEffect : GlobalToggleEffect
{
    private AdditionalEnemyGroupToggleEffectData.AdditionalEnemyGroup _additionalEnemyGroup;
    [Inject] private EnemySpawnGroupCompiler _enemySpawnGroupCompiler;
    
    public void SetAdditionalEnemyGroup(AdditionalEnemyGroupToggleEffectData.AdditionalEnemyGroup additionalEnemyGroup)
    {
        _additionalEnemyGroup = additionalEnemyGroup;
    }
    
    public override void Enable()
    {
        _enemySpawnGroupCompiler.AddAdditionalEnemyGroup(_additionalEnemyGroup);
    }

    public override void Disable()
    {
        _enemySpawnGroupCompiler.RemoveAdditionalEnemyGroup(_additionalEnemyGroup);
    }
}