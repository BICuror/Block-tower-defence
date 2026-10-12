using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using Zenject;
using System;

namespace Combat
{
    public sealed class BuildingEntity : CombatEntity
    {
        [Inject] private WaveStateMachine _waveStateMachine;
         
        [Header("Data")]
        [SerializeField] private List<EntityModifcatorTag> _defaultBuildingTags;
        [SerializeField] private BuildingEntityType _buildingEntityType;
        [SerializeField] private BuildingAttackType _buildingAttackType; 
        
        [Header("DestroyedObject")]
        [SerializeField] private bool _leavesBuildingDestroyedObject = true;
        [ShowIf("_leavesBuildingDestroyedObject")] [SerializeField] private bool _leavesDestroyedObjectInIdleState = true;
        [ShowIf("_leavesBuildingDestroyedObject")] [SerializeField] private BuildingDestroyedObject _buildingDestroyedObjectPrefab;
        private BuildingDestroyedObject _currentDestroyedObject;
        
        private BuildingHealth _health;

        public DraggableObject Draggable => ComponentsContainer.Get<DraggableObject>();
        public List<EntityModifcatorTag> DefaultBuildingTags => _defaultBuildingTags;
        public BuildingEntityType BuildingEntityType => _buildingEntityType;
        public BuildingAttackType BuildingAttackType => _buildingAttackType;
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

        protected override void OnHandleDeath()
        {
            if (_leavesBuildingDestroyedObject)
            {
                if (!_leavesDestroyedObjectInIdleState && _waveStateMachine.CurrentState == WaveState.Idle) return;
                
                _currentDestroyedObject = Instantiate(_buildingDestroyedObjectPrefab, transform.position, transform.rotation);
                _currentDestroyedObject.GetComponent<DraggableObject>().SetTileScale(Draggable.TileScale);
                _currentDestroyedObject.SetEntity(this, DeathHandleMethod == EntityDeathHandleMethod.Destroy);
            }
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