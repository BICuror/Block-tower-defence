using Combat.EnemySpawnSystem;
using Zenject;

public sealed class EnemiesHaveStateffectApperanceCondition : EffectApperanceCondition
{
    [Inject] private EnemySpawnGroupContainer _enemySpawnGroupContainer;
    
    public override bool CanAppear()
    {
        string requiredTypeName = Args.GetArgument<string>("RequiredTypeName");
        
        return _enemySpawnGroupContainer.GetAllEnemyConfigs().Exists(enemyData => enemyData.StatInitializers.Exists(statInitializer => statInitializer.StatData.GetStatType().Name == requiredTypeName));
    }
}