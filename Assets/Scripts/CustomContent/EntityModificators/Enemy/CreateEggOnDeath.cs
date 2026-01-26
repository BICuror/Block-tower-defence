using Cysharp.Threading.Tasks;
using UnityEngine;
using Combat;

public sealed class CreateEggOnDeath : EntityModificator
{
    private EnemyEgg _enemyEggPrefab;
    
    public override bool CanBeApplied() => !Entity.ComponentsContainer.Has<EnemyEgg>();
    
    public override void Enable()
    {
        _enemyEggPrefab = Args.GetArgument<GameObject>("EnemyEggPrefab").GetComponent<EnemyEgg>();
        
        Entity.Health.Died += SpawnEgg;
    }

    public override void Disable()
    {
        Entity.Health.Died -= SpawnEgg;
    }

    private void SpawnEgg()
    {
        EnemyEgg egg = Object.Instantiate(_enemyEggPrefab, Entity.transform.position, Quaternion.identity);
        
        egg.TrySpawnEnemy(Entity.ComponentsContainer.Get<EnemyBootstrap>().EnemyData).Forget();
    }
}