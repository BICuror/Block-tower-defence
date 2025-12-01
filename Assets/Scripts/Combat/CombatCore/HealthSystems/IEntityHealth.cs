using System;

namespace Combat
{
    public interface IHealth
    {
        public float GetMaxHp();
        public float GetHp();
        public float GetHpPercent();
        public bool IsAlive();
        public bool IsFullHp();
        
        public void ReceiveEnemyDamage(float baseDamage, CombatEntity damageDealer);
        public void ReceiveEffectDamage(float damage);
    
        public void ReceiveHeal(float heal);
        
        public void Die();
    }
}