using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Combat;

public sealed class BombsLeaveBlockTowers : EntityModificator
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [Inject] private WaveStateMachine _waveStateMachine;
    private CreateBlockTower _createBlockTowerBehaviour;
    private BombCreatorTower _bombCreatorTower;
    
    public override void Enable()
    {
        _bombCreatorTower = Entity.ComponentsContainer.Get<BombCreatorTower>();

        _createBlockTowerBehaviour = new CreateBlockTower();
        _createBlockTowerBehaviour.SetArgumentsContainer(Args);
        _createBlockTowerBehaviour.Initialize(_globalBuildingContainer, _waveStateMachine);
        
        _bombCreatorTower.BombExploded.AddBehaviour(_createBlockTowerBehaviour, BehaviourType.Additional);
        
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateStarted += _createBlockTowerBehaviour.DestroyAllCreatedTowers;
    }
    
    public override void Disable()
    {
        _bombCreatorTower.BombExploded.RemoveAdditionalBehaviour(_createBlockTowerBehaviour);
        
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateStarted -= _createBlockTowerBehaviour.DestroyAllCreatedTowers;
    }
    
    private sealed class CreateBlockTower : CombatBehaviour<Vector3>
    {
        private GlobalBuildingContainer _globalBuildingContainer;
        private WaveStateMachine _waveStateMachine;
        private BuildingEntity _blockTowerPrefab;
        private List<BuildingEntity> _createdTowers = new();

        protected override void OnOwnerEntitySet()
        {
            _blockTowerPrefab = Args.GetArgument<GameObject>("BlockTowerPrefab").GetComponent<BuildingEntity>();
        }

        public void Initialize(GlobalBuildingContainer globalBuildingContainer, WaveStateMachine waveStateMachine)
        {
            _globalBuildingContainer = globalBuildingContainer;
            _waveStateMachine = waveStateMachine;
        } 
        
        public override void Execute(Vector3 explosionPosition)
        {
            if (_waveStateMachine.CurrentState != WaveState.Attack) return;
            
            BuildingEntity createdTower = Object.Instantiate(_blockTowerPrefab, explosionPosition, Quaternion.identity);
            
            _globalBuildingContainer.Add(createdTower);
            _createdTowers.Add(createdTower);
        }
        
        public void DestroyAllCreatedTowers()
        {
            _createdTowers.ForEach(createdTower => createdTower.Health.Die());
            _createdTowers.Clear();
        }
    }
}