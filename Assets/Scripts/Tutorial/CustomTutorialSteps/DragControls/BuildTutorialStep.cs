using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Combat;

namespace Tutorial.Custom
{
    public sealed class BuildTutorialStep : ProgressTutorialStep
    {
        [Inject] private GlobalBuildingContainer _globalBuildingContainer;
        
        [SerializeField] private float _buildingBuildTime = 2f;
        
        private BuildingEntity _buildingEntity;
        
        public override async UniTask StartStep()
        {
            _buildingEntity = _globalBuildingContainer.Entities[0];
            
            _buildingEntity.StatContainer.Get<BuildTime>().SetDefault(_buildingBuildTime);
            _buildingEntity.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted += IncreaseProgress; 
            
            await EnableUI();
        }

        public override async UniTask EndStep()
        {
            _buildingEntity.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted -= IncreaseProgress; 
            
            await DisableUI();
        }
    }
}