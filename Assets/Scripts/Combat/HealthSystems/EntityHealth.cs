using Cashing;
using System;

namespace Combat
{
    public abstract class EntityHealth : IHealth
    {
        [Cached] private CombatEntity _entity;
        [Cached] private MaxHealth _maxHpStat;
        private float _maxHealth;
        private float _currentHp;
        
        public Action Damaged;
        public Action Healed;
        public Action Died;
        public Action<CombatEntity> EntityDied; 
        
        public void Initialize()
        {
            _maxHealth = _maxHpStat.Value;
            _maxHpStat.ValueChanged += _ => ClampCurrentHpByMax();
        }
        public void RefilHP() => ReceiveHeal(_maxHealth);
        public float GetMaxHp() => _maxHealth;
        public float GetHp() => _currentHp;
        public float GetHpPercent() => _currentHp / _maxHealth;
        public bool IsAlive() => _currentHp > 0;
        public bool IsFullHp() => _currentHp == _maxHealth;

        private void ClampCurrentHpByMax()
        {
            float healthPercent = GetHpPercent();

            _maxHealth = _maxHpStat.Value;

            _currentHp = _maxHealth * healthPercent;
        }
        
        #region DamageRecivement 
        public void ReceivePercentEffectDamage(float percent) => ReceiveDamage(_maxHealth * percent);

        public void ReceiveEnemyDamage(float baseDamage, CombatEntity damageDealer)
        {
            float outDamage = damageDealer.DamageModifierContainer.DealerContainer.Modify(baseDamage, _entity);
            
            float resultDamage = _entity.DamageModifierContainer.ReciverContainer.Modify(outDamage, damageDealer);
            
            ReceiveDamage(resultDamage);

            if (!IsAlive())
            {
                damageDealer.DamageModifierContainer.InvokeOnKillEffects(_entity);
                _entity.DamageModifierContainer.InvokeOnDeathEffects(damageDealer);
            }
        }
        public void ReceiveEffectDamage(float damage) => ReceiveDamage(damage);
        private void ReceiveDamage(float damage)
        {
            if (damage == 0 || !IsAlive()) return;
            
            _currentHp -= damage;
    
            if (_currentHp <= 0) Die();
            else Damaged?.Invoke();
        }
        #endregion
        
        #region HealRecivement
        public void ReceivePercentHeal(float percent) => ReceiveHeal(percent * _maxHealth);
        public void ReceiveHeal(float heal)
        {
            if (heal == 0) return;
            
            if (_currentHp + heal < _maxHealth) _currentHp += heal;
            else _currentHp = _maxHealth;
            
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