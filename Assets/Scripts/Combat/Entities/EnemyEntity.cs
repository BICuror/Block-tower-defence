using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(ContactDamage))]
    
    public sealed class EnemyEntity : CombatEntity
    {
        [SerializeField] private bool _shouldDestoryOnDeath;
        
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
            _health.HandleDeath += HandleDeathEvent;
        }
        
        private void HandleDeathEvent()
        {
            gameObject.SetActive(false);
            
            if (_shouldDestoryOnDeath) Destroy(gameObject);
        }
    }
}