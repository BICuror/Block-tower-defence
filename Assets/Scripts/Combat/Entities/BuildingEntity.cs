namespace Combat
{
    public sealed class BuildingEntity : CombatEntity
    {
        private BuildingHealth _health;

        public BuildingHealth BuildingHealth => _health;

        private void Awake()
        {
            _health = new();
            ComponentsContainer.Add<BuildingHealth>(_health);
            ComponentsContainer.Add<EntityHealth>(_health);
            
            base.Awake();
            
            InjectCached(_health);
            _health.Initialize();
            _health.RefilHP();
            _health.Died += HandleDeathEvent;
        }

        private void HandleDeathEvent()
        {
            Destroy(gameObject);
        }
    }
}