using System.Collections.Generic;
using System.Data;
using System;
using Combat;

public sealed class GlobalEnemyContainer
{
    private readonly List<EnemyEntity> _globalEnemyEntities = new();

    public IReadOnlyList<EnemyEntity> Entities => _globalEnemyEntities;
    
    public Action<EnemyEntity> EnemyAdded;
    public Action<EnemyEntity> EnemyRemoved;
    
    public void Add(EnemyEntity enemyEntity)
    {
        if (_globalEnemyEntities.Contains(enemyEntity)) throw new DuplicateNameException();

        enemyEntity.EnemyHealth.EnemyDied  += RemoveUponDestroyment;
        
        _globalEnemyEntities.Add(enemyEntity);
        
        EnemyAdded?.Invoke(enemyEntity);
    }

    private void RemoveUponDestroyment(EnemyEntity enemyEntity)
    {
        enemyEntity.EnemyHealth.EnemyDied -= RemoveUponDestroyment;
        
        _globalEnemyEntities.Remove(enemyEntity);
        
        EnemyRemoved?.Invoke(enemyEntity);
    }
}