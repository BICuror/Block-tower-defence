using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Combat;

public sealed class StartBuildingUpgradeSelectionRewardEffect : RewardEffect
{
    [Inject] private SelectionManager _selectionManager;
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    
    public override void GrantReward()
    {
        if (_globalBuildingContainer.Entities.Count > 0)
        {
            IReadOnlyList<BuildingEntity> buildings = _globalBuildingContainer.Entities;
            BuildingEntity building = buildings[Random.Range(0, buildings.Count)];
            
            _selectionManager.EnqeueSelection(new SelectionSettings(SelectionType.BuildingUpgrade, building));
        }
        else
        {
            _selectionManager.EnqeueSelection(new SelectionSettings(SelectionType.Building));
        }
    }
}