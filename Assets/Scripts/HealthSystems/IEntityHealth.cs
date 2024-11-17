namespace Combat
{
    public interface IHealth
    {
        public float GetMaxHp();
        public float GetHp();
        public float GetHpPercent();
        public bool IsAlive();
        public bool IsFullHp();
        
        public void ReceivePercentDamage(float percent);
        public void ReceiveDamage(float damage);
    
        public void ReceivePercentHeal(float percent);
        public void ReceiveHeal(float heal);
        
        public void Die();
    }
}