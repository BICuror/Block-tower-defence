using Cashing;
using System;

namespace Combat
{
    public sealed class EnemyHealth : EntityHealth
    {
        [Cached] private EnemyEntity _enemyEntity;
    
        public event Action<EnemyEntity> EnemyDied; 
        
        public override void Die()
        {
            EnemyDied?.Invoke(_enemyEntity);
            base.Die();
        }
    }
}