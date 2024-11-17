using System;
using Cashing;
using UnityEngine;

namespace Combat
{
    public abstract class EntityHealth : MonoBehaviour, IHealth
    {
        [Cached] private CombatEntity _entity;
        private float _maxHp;
        private float _currentHp;
        
        public Action Damaged;
        public Action Healed;
        public Action<CombatEntity> EntityDied; 
        
        public float GetMaxHp() => _maxHp;
        public float GetHp() => _currentHp;
        public float GetHpPercent() => _currentHp / _maxHp;
        public bool IsAlive() => _currentHp > 0;
        public bool IsFullHp() => _currentHp == _maxHp;

        #region DamageRecivement 
        public void ReceivePercentDamage(float percent) => ReceiveDamage(percent * _maxHp);
        public void ReceiveDamage(float damage)
        {
            _currentHp -= damage;
    
            if (_currentHp <= 0) Die();
            else Damaged?.Invoke();
        }
        #endregion
        
        #region HealRecivement
        public void ReceivePercentHeal(float percent) => ReceiveHeal(_maxHp * percent);
        public void ReceiveHeal(float heal)
        {
            if (_currentHp + heal <= _maxHp) _currentHp += heal;
            else _currentHp = _maxHp;
            
            Healed?.Invoke();
        }
        #endregion

        /*#region HealthBar
        public void DisableHealthBar() => _healthBar.gameObject.SetActive(false);
        public void EnableHealthBar()
        {
            _healthBar.gameObject.SetActive(true);
    
            float currentHealth = GetHealthPrcentage();
    
            _healthBar.SetValue(currentHealth);
        }
        #endregion*/
        
        
        public virtual void Die()
        {
            EntityDied?.Invoke(_entity);
        }
    }   
}

