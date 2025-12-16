using UnityEngine;
using Navigation;
using Cashing;

namespace Combat
{
    public sealed class EnemyBootstrap : MonoBehaviour
    {
        [SerializeField] private GPUInstanceEnabler gpuInstanceEnabler;
        [SerializeField] private MeshRenderer _meshRenderer;   
        [SerializeField] private MeshFilter _meshFilter;

        [Cached] private EntityObjectModificatorContainer _entityObjectModificatorContainer;
        [Cached] private NavigationAgent _navMeshAgent;
        [Cached] private StatContainer _statContainer;
        [Cached] private CombatEntity _combatEntity;
        [Cached] private EnemyHealth _enemyHealth;
        [Cached] private Inspectable _inspectable;
        [Cached] private HealthBar _healthBar;
        [Cached] private Collider _collider;
        
        private EnemyData _enemyData;

        public EnemyData EnemyData => _enemyData;

        private void Start()
        {
            _enemyHealth.Died += OnEnemyDeath;
        }
        
        public void SetEnemyData(EnemyData enemyDataToSet, bool initializeNavigation = true, bool initializeSpecialObjects = true)
        {
            _enemyData = enemyDataToSet;

            SetStats();
            SetVisualData();
            
            _enemyHealth.RefilHP();
            _collider.enabled = true;
            
            _inspectable.SetInspectableData(enemyDataToSet.Name, enemyDataToSet.Description);

            if (initializeSpecialObjects) CreateSpecialObject();
            
            if (initializeNavigation)
            {
                _navMeshAgent.SetAgentData(enemyDataToSet.NavigationData);
                _navMeshAgent.Enable();
            }
        }

        private void SetStats()
        {
            MaxHealth maxHealthStat = _statContainer.Get<MaxHealth>(); 
            maxHealthStat.Reset(); 
            maxHealthStat.SetDefault(_enemyData.MaxHealth);
            
            Speed speedStat = _statContainer.Get<Speed>(); 
            speedStat.Reset(); 
            speedStat.SetDefault(_enemyData.Speed);
            
            ContactDamage contactDamageStat = _statContainer.Get<ContactDamage>();
            contactDamageStat.Reset();
            contactDamageStat.SetDefault(_enemyData.ContactDamage);
            
            _statContainer.AddStats(_enemyData.StatInitializers.ToArray());
        }
    
        private void SetVisualData()
        {
            _meshFilter.sharedMesh = _enemyData.Mesh;
            _meshRenderer.sharedMaterial = _enemyData.Material;
            gpuInstanceEnabler.EnableGPUInstancing();
        }
    
        private void CreateSpecialObject()
        {
            if (_enemyData.HasObjectModificators)
            {
                _enemyData.ObjectModificators.ForEach(additionalObjectPrefab =>
                {
                    _entityObjectModificatorContainer.InstantiateAndAddModificator(additionalObjectPrefab);
                });
            }
        }

        private void OnEnemyDeath()
        {
            _statContainer.RemoveStats(_enemyData.StatInitializers.ToArray());
            _entityObjectModificatorContainer.DestroyAllModificators();
            _navMeshAgent.Disable();
        }

        private void OnDisable()
        {
            _collider.enabled = false;
        }
    }
}