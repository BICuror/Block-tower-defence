using System;
using Cashing;
using UnityEngine;

namespace Combat
{
    public class EnemyHealth : EntityHealth
    {
        [Cached] private EnemyEntity _enemyEntity;
        [SerializeField] private float _attackDamage = 10f;
    
        public Action<EnemyEntity> EnemyDied; 
    
        private void OnCollisionEnter(Collision other) 
        {
            if (other.gameObject.TryGetComponent(out BuildingHealth buildingHealth))
            {
                buildingHealth.ReceiveDamage(_attackDamage);
    
                Die();
            }    
        }
    
        public override void Die()
        {
            base.Die();
            EnemyDied.Invoke(_enemyEntity);
            gameObject.SetActive(false);
        }
    }
}
