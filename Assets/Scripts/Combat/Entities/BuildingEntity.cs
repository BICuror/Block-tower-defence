using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Combat
{
    public sealed class BuildingEntity : CombatEntity
    {
        [Header("Data")]
        [SerializeField] private List<EntityModifcatorTag> _defaultBuildingTags;
        [SerializeField] private BuildingEntityType _buildingEntityType;
        [SerializeField] private BuildingAttackType _buildingAttackType; 
        
        [Header("DestroyedObject")]
        [SerializeField] private bool _destroyOnDeath;
        [SerializeField] private bool _leavesBuildingDestroyedObject = true;
        [ShowIf("_leavesBuildingDestroyedObject")] [SerializeField] private BuildingDestroyedObject _buildingDestroyedObjectPrefab;
        private BuildingDestroyedObject _currentDestroyedObject;
        
        private BuildingHealth _health;

        public List<EntityModifcatorTag> DefaultBuildingTags => _defaultBuildingTags;
        public BuildingEntityType BuildingEntityType => _buildingEntityType;
        public BuildingAttackType BuildingAttackType => _buildingAttackType;
        public bool IsDestroyedOnDeath => _destroyOnDeath;
        public BuildingHealth BuildingHealth => _health;
        
        public event Action<BuildingEntity> BuildingRevived;

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
        
        public async UniTask ReviveBuilding()
        {
            if (_currentDestroyedObject)
            {
                await UniTask.WaitUntil(() => _currentDestroyedObject.CanBeRevived);

                transform.position = _currentDestroyedObject.transform.position;
                _currentDestroyedObject.Destroy().Forget();
            }
            
            gameObject.SetActive(true);
            Health.ReceivePercentHeal(0.1f);
            BuildingRevived?.Invoke(this);
        }

        private void HandleDeathEvent()
        {
            gameObject.SetActive(false);

            if (_leavesBuildingDestroyedObject)
            {
                _currentDestroyedObject = Instantiate(_buildingDestroyedObjectPrefab, transform.position, transform.rotation);
                _currentDestroyedObject.SetEntity(this, _destroyOnDeath);
            }

            if (_destroyOnDeath) Destroy(gameObject);
        }
    }
}

//used for perk selection
public enum BuildingEntityType
{
    None = 0,
    Archery = 1,
    Mortar = 2,
    Inferno = 3,
    Lightning = 4,
    Orbital = 5,
    BombTower = 6,
    Artilery = 7,
    Brawler = 8,
}

//primararly used for selection of first waves, where only one building is available
public enum BuildingAttackType
{
    None,
    Area,
    Single,
    Group
}