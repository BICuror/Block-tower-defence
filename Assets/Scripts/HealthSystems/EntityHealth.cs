using UnityEngine;
using Cashing;
using System;

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
        public void ReceiveEnemyDamage(float damage, CombatEntity damageDealer) => ReceiveDamage(damage, damageDealer);
        public void ReceiveEffectDamage(float damage) => ReceiveDamage(damage, null);
        private void ReceiveDamage(float damage, CombatEntity damageDealer)
        {
            damage = _entity.DamageModifierContainer.ReciverContainer.Modify(damage, damageDealer);
            
            if (damage == 0 || !IsAlive()) return;
            
            _currentHp -= damage;
    
            if (_currentHp <= 0) Die();
            else Damaged?.Invoke();
        }
        #endregion
        
        #region HealRecivement
        public void ReceiveHeal(float heal)
        {
            if (heal == 0) return;
            
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