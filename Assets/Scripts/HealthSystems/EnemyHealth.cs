using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : EntityHealth
{
    [SerializeField] private float _attackDamage = 10f;

    public UnityEvent<EnemyHealth> EnemyDeathEvent; 

    private float _attackMultipluer;

    public void SetEnemyData(EnemyHealthData enemyData)
    {
        _maxHealth = enemyData.MaxHealth;

        _incomingDamageMultipluer = enemyData.IncomingDamageMultipluer;

        EnableHealthBar();

        HealFully();
    }

    public void MultiplyMaxHealth(float value) 
    {
        _maxHealth *= value;
        
        HealFully();
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.TryGetComponent(out BuildingHealth buildingHealth))
        {
            buildingHealth.GetHurt(_attackDamage);

            Die();
        }    
    }

    public override void Die()
    {
        DeathEvent.Invoke(gameObject);

        EnemyDeathEvent.Invoke(this);

        gameObject.SetActive(false);
    }
}
