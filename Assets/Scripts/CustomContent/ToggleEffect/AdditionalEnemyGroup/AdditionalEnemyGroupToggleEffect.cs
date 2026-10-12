using Combat.EnemySpawnSystem;
using Zenject;

public sealed class AdditionalEnemyGroupToggleEffect : GlobalEffect
{
    [Inject] private EnemySpawnGroupCompiler _enemySpawnGroupCompiler;
    
    public override void Enable()
    {
        _enemySpawnGroupCompiler.AddAdditionalEnemyGroup(Args.GetArgument<AdditionalEnemyGroupDataConfig>("EnemyGroupData"));
    }

    public override void Disable()
    {
        _enemySpawnGroupCompiler.RemoveAdditionalEnemyGroup(Args.GetArgument<AdditionalEnemyGroupDataConfig>("EnemyGroupData"));
    }
}