using System;
using Cashing;
using UnityEngine;

namespace Combat
{
    public abstract class EntityHealth : MonoBehaviour, IHealth
    {
        [Cached] private CombatEntity _entity;
        [Cached] private MaxHealth _maxHpStat;
        private float _currentHp;
        
        public Action Damaged;
        public Action Healed;
        public Action<CombatEntity> EntityDied; 
        
        private void Start() => Initialize();
        public void Initialize()
        {
            _currentHp = _maxHpStat.Value;
        }
        public float GetMaxHp() => _maxHpStat.Value;
        public float GetHp() => _currentHp;
        public float GetHpPercent() => _currentHp / _maxHpStat.Value;
        public bool IsAlive() => _currentHp > 0;
        public bool IsFullHp() => _currentHp == _maxHpStat.Value;

        #region DamageRecivement 
        public void ReceivePercentDamage(float percent) => ReceiveDamage(percent * _maxHpStat.Value);
        public void ReceiveDamage(float damage)
        {
            _currentHp -= damage;
    
            if (_currentHp <= 0) Die();
            else Damaged?.Invoke();
        }
        #endregion
        
        #region HealRecivement
        public void ReceivePercentHeal(float percent) => ReceiveHeal(_maxHpStat.Value * percent);
        public void ReceiveHeal(float heal)
        {
            if (_currentHp + heal <= _maxHpStat.Value) _currentHp += heal;
            else _currentHp = _maxHpStat.Value;
            
            Healed?.Invoke();
        }
        #endregion
        
        public virtual void Die()
        {
            EntityDied?.Invoke(_entity);
        }
    }   
}