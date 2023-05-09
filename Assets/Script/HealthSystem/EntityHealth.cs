using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    private float _currentHealth;

    public UnityEvent HealthChanged;
    
    public UnityEvent<GameObject> DestroyEvent; 

    [SerializeField] protected HealthBar _healthBar;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }
    
    public float GetMaxHealth() => _maxHealth;
    public float GetHealthPrcentage() => _currentHealth / _maxHealth;

    public void MultiplyHealth(float multiplyer)
    {
        _maxHealth *= multiplyer;
        _currentHealth *= multiplyer;
    }

    public void GetHurt(float damage)
    {
        _currentHealth -= damage;

        HealthChanged?.Invoke();

        _healthBar.UpdateValue(GetHealthPrcentage());

        if (_currentHealth <= 0) 
        {
            Die();
        }
    }
    
    public void Heal(float healAmount)
    {
        if (_currentHealth + healAmount <= _maxHealth)
        {
            _currentHealth += healAmount;
        }
        else
        { 
            _currentHealth = _maxHealth;
        }

        HealthChanged?.Invoke();
        _healthBar.UpdateValue(GetHealthPrcentage());
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        DestroyEvent?.Invoke(gameObject);
    }
}   
