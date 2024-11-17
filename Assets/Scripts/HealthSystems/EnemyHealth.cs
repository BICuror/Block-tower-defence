using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class EnemyHealth : EntityHealth
    {
        [SerializeField] private float _attackDamage = 10f;
    
        public UnityEvent<EnemyHealth> EnemyDeathEvent; 
    
        private float _attackMultipluer;
    
        public void SetEnemyData(EnemyHealthData enemyData)
        {
            ReceivePercentHeal(1f);
        }
    
        public void MultiplyMaxHealth(float value) 
        {
            ReceivePercentHeal(1f);
        }
    
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
            EnemyDeathEvent.Invoke(this);
    
            gameObject.SetActive(false);
        }
    }
}
