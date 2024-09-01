using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] protected float _maxHealth;
    private float _currentHealth;

    protected float _incomingDamageMultipluer = 1f;

    private bool _isInvinsible;

    public UnityEvent Damaged;

    public UnityEvent Healed;
    
    public UnityEvent<GameObject> DeathEvent; 

    [SerializeField] protected HealthBar _healthBar;

    protected void Awake() => _currentHealth = _maxHealth;

    public float GetCurrentHealth() => _currentHealth;
    public float GetHealthPrcentage() => _currentHealth / _maxHealth;
    public bool IsAlive() => _currentHealth > 0;
    public float GetMaxHealth() => _maxHealth;

    public void ChangeIncomingDamageMultipluer(float value) => _incomingDamageMultipluer += value;
    public void SetInvincibleState(bool state) => _isInvinsible = state;

    #region HealthBar
    public void DisableHealthBar() => _healthBar.gameObject.SetActive(false);
    public void EnableHealthBar()
    {
        _healthBar.gameObject.SetActive(true);

        float currentHealth = GetHealthPrcentage();

        _healthBar.SetValue(currentHealth);
    }
    #endregion

    public void GetHurtPrecent(float precent)
    {
        GetHurt(precent * GetMaxHealth());
    }

    public virtual void GetHurt(float damage)
    {
        if (_isInvinsible) return;

        float healthDifference = GetHealthPrcentage();

        damage *= _incomingDamageMultipluer;

        _currentHealth -= damage;

        if (_currentHealth <= 0) 
        {
            Die();
        }
        else
        {
            Damaged.Invoke();
            
            _healthBar.DecreaseValue(GetHealthPrcentage(), healthDifference);
        }
    }
    #region Heal
    public void HealFully() => Heal(_maxHealth - _currentHealth);
    public void HealByPercent(float value) => Heal(_maxHealth * value);
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

        Healed?.Invoke();

        _healthBar.IncreaseValue(GetHealthPrcentage());
    }
    #endregion

    public virtual void Die()
    {
        DeathEvent.Invoke(gameObject);
        
        Destroy(gameObject);
    }
}   
