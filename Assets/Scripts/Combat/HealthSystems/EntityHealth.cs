using Cashing;
using System;

namespace Combat
{
    public abstract class EntityHealth : IHealth
    {
        [Cached] private CombatEntity _entity;
        [Cached] private MaxHealth _maxHpStat;
        private float _currentHp;
        
        public Action Damaged;
        public Action Healed;
        public Action Died;
        public Action<CombatEntity> EntityDied; 
        
        public void Initialize()
        {
            _maxHpStat.ValueChanged += _ => ClampCurrentHpByMax();
        }
        public void RefilHP() => _currentHp = _maxHpStat.Value;
        public float GetMaxHp() => _maxHpStat.Value;
        public float GetHp() => _currentHp;
        public float GetHpPercent() => _currentHp / _maxHpStat.Value;
        public bool IsAlive() => _currentHp > 0;
        public bool IsFullHp() => _currentHp == _maxHpStat.Value;

        private void ClampCurrentHpByMax()
        {
            if (_currentHp > _maxHpStat.Value) _currentHp = _maxHpStat.Value;
        }
        
        #region DamageRecivement 
        public void ReceivePercentEffectDamage(float percent) => ReceiveDamage(_maxHpStat.Value * percent, null);
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
        public void ReceivePercentHeal(float percent) => ReceiveHeal(percent * _maxHpStat.Value);
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
            _currentHp = 0;
            Died?.Invoke();
            EntityDied?.Invoke(_entity);
        }
    }   
}