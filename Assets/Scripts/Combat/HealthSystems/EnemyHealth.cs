using Cashing;
using System;

namespace Combat
{
    public sealed class EnemyHealth : EntityHealth
    {
        [Cached] private EnemyEntity _enemyEntity;
    
        public event Action<EnemyEntity> EnemyDied; 
        
        protected override void InvokeEntityDied()
        {
            EnemyDied?.Invoke(_enemyEntity);
        }
    }
}