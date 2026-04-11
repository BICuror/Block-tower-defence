using System;
using Combat.Animation;
using UnityEngine;
using Navigation;
using Cashing;
using CuroAudio;

namespace Combat
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
        
        private EnemyData _enemyData;

        public EnemyData EnemyData => _enemyData;

        private void Start()
        {
            _enemyHealth.HandleDeath += OnEnemyDeath;
        }
        
        public void SetEnemyData(EnemyData enemyDataToSet, bool initializeNavigation = true, bool initializeModificators = true)
        {
            _enemyData = enemyDataToSet;

            SetStats();
            SetVisualData();
            
            _enemyHealth.RefilHP();
            _collider.enabled = true;
            
            _inspectableObject.SetLocalizationKey(enemyDataToSet.LocalizationKey);

            ApplyEntityEffectImmunities();

            if (initializeModificators)
            {
                TryCreateSpecialObjects();
                TryApplyEntityModificators();
            }
            
            if (initializeNavigation)
            {
                _navMeshAgent.SetAgentData(enemyDataToSet.NavigationData);
                _navMeshAgent.Enable();
            }
        }

        #region ApplyData

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
            
            _enemyContactDamager.SetIsDestroyedOnContactDamage(_enemyData.DiesOnContact);
        }
    
        private void SetVisualData()
        {
            _meshFilter.sharedMesh = _enemyData.Mesh;
            
            _hitHighlighter.SetDefaultMaterial(_enemyData.Material);

            _hitShaker.SetDefaultValues(new Vector3(_enemyData.Scale, _enemyData.Scale, _enemyData.Scale));
        }
    
        private void TryCreateSpecialObjects()
        {
            if (!_enemyData.HasObjectModificators) return;
            
            _enemyData.ObjectModificators.ForEach(additionalObjectPrefab =>
            {
                _entityObjectModificatorContainer.InstantiateAndAddModificator(additionalObjectPrefab);
            });
        }

        private void TryApplyEntityModificators()
        {
            if (!_enemyData.HasEntityModificators) return;
            
            _enemyData.EntityModificatorDatas.ForEach(entityModificatorData =>
            {
                _combatEntity.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(entityModificatorData);
            });
        }
        
        private void ApplyEntityEffectImmunities()
        {
            _enemyData.EffectImmunities.ForEach(effectType =>
            {
                Type entityEffectType = Type.GetType(effectType);
                
                _combatEntity.ComponentsContainer.Get<EntityEffectManager>().AddEffectImmunity(entityEffectType);
            });
        }

        #endregion

        #region RemoveData

        private void TryRemoveEntityModificators()
        {
            if (!_enemyData.HasEntityModificators) return;
            
            _enemyData.EntityModificatorDatas.ForEach(entityModificatorData =>
            {
                _combatEntity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(entityModificatorData);
            });
        }

        private void RemoveEntityEffectImmunities()
        {
            _enemyData.EffectImmunities.ForEach(effectType =>
            {
                Type entityEffectType = Type.GetType(effectType);
                
                _combatEntity.ComponentsContainer.Get<EntityEffectManager>().RemoveEffectImmunity(entityEffectType);
            });
        }
        
        #endregion

        private void OnEnemyDeath()
        {
            AudioSystem.PlaySFX(_enemyData.DeathSound, transform.position);
            
            TryRemoveEntityModificators();
            _entityObjectModificatorContainer.DestroyAllModificators();
            _statContainer.RemoveStats(_enemyData.StatInitializers.ToArray());
            RemoveEntityEffectImmunities();
            
            _navMeshAgent.Disable();
        }

        private void OnDisable()
        {
            _collider.enabled = false;
        }
    }
}