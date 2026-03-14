using NaughtyAttributes;
using UnityEngine;

namespace Combat
{
    public sealed class BuildingEntity : CombatEntity
    {
        [SerializeField] private bool _destroyOnDeath;
        
        [Header("DestroyedObject")]
        [SerializeField] private bool _leavesBuildingDestroyedObject = true;
        [ShowIf("_leavesBuildingDestroyedObject")] [SerializeField] private BuildingDestroyedObject _buildingDestroyedObjectPrefab;
        
        private BuildingHealth _health;

        public bool IsDestroyedOnDeath => _destroyOnDeath;
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
            _health.HandleDeath += HandleDeathEvent;
        }

        private void HandleDeathEvent()
        {
            gameObject.SetActive(false);

            if (_leavesBuildingDestroyedObject)
            {
                BuildingDestroyedObject buildingDestroyedObject = Instantiate(_buildingDestroyedObjectPrefab, transform.position, transform.rotation);
                buildingDestroyedObject.SetEntity(this, _destroyOnDeath);
            }

            if (_destroyOnDeath) Destroy(gameObject);
        }
    }
}