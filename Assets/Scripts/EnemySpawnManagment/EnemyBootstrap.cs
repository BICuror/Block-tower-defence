using UnityEngine;
using Navigation;
using Cashing;

namespace Combat
{
    public sealed class EnemyBootstrap : MonoBehaviour
    {
        [SerializeField] private GPUInstancerEnabler _GPUInstancerEnabler;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;   
        [SerializeField] private Animator _animator;

        [Cached] private CombatEntity _combatEntity;
        [Cached] private NavigationAgent _navMeshAgent;
        [Cached] private EnemyHealth _enemyHealth;
        [Cached] private StatContainer _statContainer;
        [Cached] private HealthBar _healthBar;
        [Cached] private EntityObjectModificatorContainer _entityObjectModificatorContainer;
        [Cached] private Collider _collider;
        
        private EnemyData _enemyData;
    
        public void SetEnemyData(EnemyData enemyDataToSet)
        {
            _enemyData = enemyDataToSet;

            SetStats();
            SetVisualData();
            CreateSpecialObject();
            
            _navMeshAgent.SetAgentData(enemyDataToSet.NavigationData);
            _enemyHealth.RefilHP();
            _navMeshAgent.Initialize();
            _collider.enabled = true;
        }

        private void SetStats()
        {
            MaxHealth maxHealthStat = _statContainer.Get<MaxHealth>(); 
            maxHealthStat.Reset(); 
            maxHealthStat.SetDefault(_enemyData.MaxHealth);
            
            Speed speedStat = _statContainer.Get<Speed>(); 
            speedStat.Reset(); 
            speedStat.SetDefault(_enemyData.Speed);
        }
    
        private void SetVisualData()
        {
            _meshFilter.sharedMesh = _enemyData.Mesh;
            _meshRenderer.sharedMaterial = _enemyData.Material;
            _GPUInstancerEnabler.EnableGPUInstancing();
        }
    
        private void CreateSpecialObject()
        {
            if (_enemyData.HasObjectModificators)
            {
                _enemyData.ObjectModificators.ForEach(additionalObjectPrefab =>
                {
                    _entityObjectModificatorContainer.InstantiateAndAddModificator<GameObject>(additionalObjectPrefab);
                });
            }
        }

        private void OnDisable()
        {
            _collider.enabled = false;
        }
    }
}