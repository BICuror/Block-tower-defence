using Zenject;

public sealed class EnemiesHaveStateffectApperanceCondition : EffectApperanceCondition
{
    [Inject] private EnemySpawnGroupCompiler _enemySpawnGroupCompiler; 
    
    public override bool CanAppear()
    {
        string requiredTypeName = Args.GetArgument<string>("RequiredTypeName");
        
        return _enemySpawnGroupCompiler.GetAllEnemyDatas().Exists(enemyData => enemyData.StatInitializers.Exists(statInitializer => statInitializer.StatData.GetStatType().Name == requiredTypeName));
    }
}