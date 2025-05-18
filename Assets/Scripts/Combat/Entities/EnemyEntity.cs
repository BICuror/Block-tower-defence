namespace Combat
{
    public sealed class EnemyEntity : CombatEntity
    {
        private EnemyHealth _health;
        
        public EnemyHealth EnemyHealth => _health;

        private void Awake()
        {
            _health = new();
            ComponentsContainer.Add<EnemyHealth>(_health);
            ComponentsContainer.Add<EntityHealth>(_health);
            base.Awake();
            InjectCached(_health);
            _health.Initialize();
            _health.Died += HandleDeathEvent;
        }
        
        private void HandleDeathEvent()
        {
            gameObject.SetActive(false);
        }
    }
}