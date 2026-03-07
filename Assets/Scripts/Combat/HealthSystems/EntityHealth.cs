using Cashing;
using System;

namespace Combat
{
    public abstract class EntityHealth : IHealth
    {
        [Cached] private CombatEntity _entity;
        [Cached] private MaxHealth _maxHpStat;
        
        private readonly TokenContainer _invulnerabilityTokenContainer = new(false);
        private float _maxHealth;
        private float _currentHp;
        
        public TokenContainer InvulnerabilityTokenContainer => _invulnerabilityTokenContainer;
        
        public event Action Damaged;
        public event Action Healed;
        public event Action Died;
        public event Action HandleDeath;
        public event Action<CombatEntity> EntityDamaged;
        public event Action<CombatEntity> EntityDied; 
        
        public void Initialize()
        {
            _maxHealth = _maxHpStat.Value;
            _maxHpStat.ValueChanged += _ => ClampCurrentHpByMax();
        }
        public void RefilHP() => ReceiveHeal(_maxHealth);
        public float GetMaxHp() => _maxHealth;
        public float GetHp() => _currentHp;
        public float GetHpPercent() => _currentHp / _maxHealth;
        public bool IsAlive() => _currentHp > 0f;
        public bool IsFullHp() => _currentHp == _maxHealth;

        private void ClampCurrentHpByMax()
        {
            float healthPercent = GetHpPercent();

            _maxHealth = _maxHpStat.Value;

            _currentHp = _maxHealth * healthPercent;
        }
        
        #region DamageRecivement 
        public void ReceivePercentEffectDamage(float percent) => ReceiveEffectDamage(_maxHealth * percent);
        public void ReceiveEffectDamage(float damage)
        {
            if (damage <= 0 || !IsAlive() || !_invulnerabilityTokenContainer.IsEmpty) return;
            
            ReceiveDamage(damage);
            OnDamageTaken();
        }

        public void ReceiveEnemyDamage(float baseDamage, CombatEntity damageDealer)
        {
            float outDamage = damageDealer.DamageModifierContainer.DealerContainer.ModifyByAllModificators(baseDamage, _entity);
            
            float resultDamage = _entity.DamageModifierContainer.ReciverContainer.ModifyByAllModificators(outDamage, damageDealer);
            
            if (resultDamage <= 0 || !IsAlive() || !_invulnerabilityTokenContainer.IsEmpty) return;
            
            ReceiveDamage(resultDamage);
            
            if (!IsAlive())
            {
                damageDealer.DamageModifierContainer.InvokeOnKillEffects(_entity);
                _entity.DamageModifierContainer.InvokeOnDeathEffects(damageDealer);
            }
            
            OnDamageTaken();
        }

        private void ReceiveDamage(float damage)
        {
            _currentHp -= damage;

            DamageNumberDisplayManager.Instance.DisplayDamageNumber(damage, _entity.transform.position, DamageType.Damage);
        }

        private void OnDamageTaken()
        {
            if (!IsAlive()) Die();
            else
            {
                EntityDamaged?.Invoke(_entity);
                Damaged?.Invoke();
            }
        }
        #endregion
        
        #region HealRecivement
        public void ReceivePercentHeal(float percent) => ReceiveHeal(percent * _maxHealth);
        public void ReceiveHeal(float heal)
        {
            if (heal == 0 || IsFullHp()) return;
            
            if (_currentHp + heal < _maxHealth) _currentHp += heal;
            else _currentHp = _maxHealth;
            
            Healed?.Invoke();
        }
        #endregion
        
        public void Die()
        {
            _currentHp = 0;
            Died?.Invoke();
            EntityDied?.Invoke(_entity);
            InvokeEntityDied();
            HandleDeath.Invoke();
        }

        protected abstract void InvokeEntityDied();
    }   
}