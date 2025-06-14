using UnityEngine;
using Zenject;
using Combat;

public sealed class ChanceToDropCrystalsFromEnemies : GlobalToggleEffect
{
    [Inject] private GlobalEnemyContainer _globalEnemyContainer;
    [Inject] private ItemFactory _itemFactory;
    
    public override void Enable()
    {
        _globalEnemyContainer.EnemyRemoved += TrySpawnCrystal;
    }

    private void TrySpawnCrystal(EnemyEntity enemyEntity)
    {
        if (Random.Range(0, 100) <= Args.GetArgument<int>("ChanceToDropCrystal"))
        {
            _itemFactory.CreateItem(1, 2, enemyEntity.transform.position);
        }
    }
    
    public override void Disable()
    {
        _globalEnemyContainer.EnemyRemoved -= TrySpawnCrystal;
    }
}