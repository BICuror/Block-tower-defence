using Combat.EnemySpawnSystem;
using Combat.Animation;
using Combat.IngameUI;
using UnityEngine;
using Navigation;
using CuroAudio;
using Cashing;
using System;

namespace Combat.Enemies
{
    public sealed class EnemyBootstrap : MonoBehaviour
    {
        [SerializeField] private HitHighlighter _hitHighlighter;
        [SerializeField] private MeshRenderer _meshRenderer;   
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private HitShaker _hitShaker;

        [Cached] private EntityObjectModificatorContainer _entityObjectModificatorContainer;
        [Cached] private EnemyContactDamager _enemyContactDamager;
        [Cached] private InspectableObject _inspectableObject;
        [Cached] private NavigationAgent _navMeshAgent;
        [Cached] private StatContainer _statContainer;
        [Cached] private CombatEntity _combatEntity;
        [Cached] private EnemyHealth _enemyHealth;
        [Cached] private HealthBar _healthBar;
        [Cached] private Collider _collider;
        
        private EnemyConfig _enemyConfig;

        public EnemyConfig EnemyConfig => _enemyConfig;

        private void Start()
        {
            _enemyHealth.HandleDeath += OnEnemyDeath;
        }
        
        public void SetEnemyConfig(EnemyConfig enemyConfigToSet, bool initializeNavigation = true, bool initializeModificators = true)
        {
            _enemyConfig = enemyConfigToSet;

            SetStats();
            SetVisualData();
            
            _enemyHealth.RefilHP();
            _collider.enabled = true;
            
            _inspectableObject.SetLocalizationKey(enemyConfigToSet.LocalizationKey);

            ApplyEntityEffectImmunities();

            if (initializeModificators)
            {
                TryCreateSpecialObjects();
                TryApplyEntityModificators();
            }
            
            if (initializeNavigation)
            {
                _navMeshAgent.SetAgentData(enemyConfigToSet.NavigationData);
                _navMeshAgent.Enable();
            }
        }

        #region ApplyData

        private void SetStats()
        {
            MaxHealth maxHealthStat = _statContainer.Get<MaxHealth>(); 
            maxHealthStat.Reset(); 
            maxHealthStat.SetDefault(_enemyConfig.MaxHealth);
            
            Speed speedStat = _statContainer.Get<Speed>(); 
            speedStat.Reset(); 
            speedStat.SetDefault(_enemyConfig.Speed);
            
            ContactDamage contactDamageStat = _statContainer.Get<ContactDamage>();
            contactDamageStat.Reset();
            contactDamageStat.SetDefault(_enemyConfig.ContactDamage);
            
            _statContainer.AddStats(_enemyConfig.StatInitializers.ToArray());
            
            _enemyContactDamager.SetIsDestroyedOnContactDamage(_enemyConfig.DiesOnContact);
        }
    
        private void SetVisualData()
        {
            _meshFilter.sharedMesh = _enemyConfig.Mesh;
            
            _hitHighlighter.SetDefaultMaterial(_enemyConfig.Material);

            _hitShaker.SetDefaultValues(new Vector3(_enemyConfig.Scale, _enemyConfig.Scale, _enemyConfig.Scale));
        }
    
        private void TryCreateSpecialObjects()
        {
            if (!_enemyConfig.HasObjectModificators) return;
            
            _enemyConfig.ObjectModificators.ForEach(additionalObjectPrefab =>
            {
                _entityObjectModificatorContainer.InstantiateAndAddModificator(additionalObjectPrefab);
            });
        }

        private void TryApplyEntityModificators()
        {
            if (!_enemyConfig.HasEntityModificators) return;
            
            _enemyConfig.EntityModificatorDatas.ForEach(entityModificatorData =>
            {
                _combatEntity.ComponentsContainer.Get<EntityModificatorsContainer>().AddEffect(entityModificatorData);
            });
        }
        
        private void ApplyEntityEffectImmunities()
        {
            _enemyConfig.EffectImmunities.ForEach(effectType =>
            {
                Type entityEffectType = Type.GetType(effectType);
                
                _combatEntity.ComponentsContainer.Get<EntityEffectManager>().AddEffectImmunity(entityEffectType);
            });
        }

        #endregion

        #region RemoveData

        private void TryRemoveEntityModificators()
        {
            if (!_enemyConfig.HasEntityModificators) return;
            
            _enemyConfig.EntityModificatorDatas.ForEach(entityModificatorData =>
            {
                _combatEntity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(entityModificatorData);
            });
        }

        private void RemoveEntityEffectImmunities()
        {
            _enemyConfig.EffectImmunities.ForEach(effectType =>
            {
                Type entityEffectType = Type.GetType(effectType);
                
                _combatEntity.ComponentsContainer.Get<EntityEffectManager>().RemoveEffectImmunity(entityEffectType);
            });
        }
        
        #endregion

        private void OnEnemyDeath()
        {
            AudioSystem.PlaySFX(_enemyConfig.DeathSound, transform.position);
            
            TryRemoveEntityModificators();
            _entityObjectModificatorContainer.DestroyAllModificators();
            _statContainer.RemoveStats(_enemyConfig.StatInitializers.ToArray());
            RemoveEntityEffectImmunities();
            
            _navMeshAgent.Disable();
        }

        private void OnDisable()
        {
            _collider.enabled = false;
        }
    }
}