using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Combat;

public sealed class BuildingUpgradeSelector : MonoBehaviour
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [SerializeField] private EntityModificatorDataSelector _entityModificatorDataSelector;
    [SerializeField] private DraggableConnector _draggableConnector;
    [SerializeField] private BuildingUpgradeSelectionOptionObject _buildingUpgradeSelectionOptionObjectPrefab;
    [SerializeField] private SelectionOptionObjectController _selectionOptionObjectController;
    [SerializeField] private Transform _centerPosition;
    private BuildingEntity _buildingEntityToUpgrade;
    private Vector3Int _initialPosition;

    private void Awake()
    {
        _draggableConnector.transform.SetParent(null);
        _draggableConnector.transform.localScale = Vector3.one;
    }
    
    public async UniTask StartUpgradeSelection()
    {
        _buildingEntityToUpgrade = FindBuildingsWithLeastModificators();
            
        _initialPosition = Vector3Int.RoundToInt(_buildingEntityToUpgrade.transform.position);
        
        await CaptureDraggable();
        
        List<EntityModificatorData> modifierDatas = _entityModificatorDataSelector.GetRandomEntityEffectDatas(_buildingEntityToUpgrade, 3);
        
        await _selectionOptionObjectController.CreateSelectionOptionObjects(_buildingUpgradeSelectionOptionObjectPrefab, modifierDatas.Count, InitializeSelectionOption);
        
        void InitializeSelectionOption(BuildingUpgradeSelectionOptionObject selectionOptionObject)
        {
            int prefabIndex = Random.Range(0, modifierDatas.Count);
            
            selectionOptionObject.SetTargetBuildingEntity(_buildingEntityToUpgrade);
            selectionOptionObject.SetEffectData(modifierDatas[prefabIndex]);

            modifierDatas.RemoveAt(prefabIndex);
        }
    }

    public async UniTask EndSelection()
    {
        await ReleaseDraggable();
    }
    
    private async UniTask CaptureDraggable()
    {
        await UniTask.WaitUntil(() => _buildingEntityToUpgrade.Draggable.IsPlaced);
        
        _draggableConnector.gameObject.SetActive(true);

        _draggableConnector.transform.position = _buildingEntityToUpgrade.transform.position;
        
        _draggableConnector.PickUpDraggable(_buildingEntityToUpgrade.gameObject);
        
        await _draggableConnector.MoveTo(_centerPosition.transform.position, 0.5f);
        
        await UniTask.WaitForFixedUpdate();
        
        _buildingEntityToUpgrade.transform.localPosition = Vector3.zero;
    }

    private async UniTask ReleaseDraggable()
    {
        _draggableConnector.transform.position = _centerPosition.position;

        Vector3 placementPosition = TileMap.GetNearestDraggablePlacePosition(_buildingEntityToUpgrade.ComponentsContainer.Get<BuildingDraggable>(), _initialPosition);

        await _draggableConnector.MoveTo(placementPosition, 0.2f);

        await _draggableConnector.PlaceDraggable(_buildingEntityToUpgrade.gameObject, _buildingEntityToUpgrade.ComponentsContainer.Get<BuildingDraggable>(), placementPosition);

        _draggableConnector.gameObject.SetActive(false);
    }

    private BuildingEntity FindBuildingsWithLeastModificators()
    {
        List<BuildingEntity> validBuildings = _globalBuildingContainer.GetPlayerBuildings();
        
        int minimalModificators = int.MaxValue;
        BuildingEntity foundEntity = null;
        
        foreach (BuildingEntity buildingEntity in validBuildings)
        {
            if (buildingEntity.ComponentsContainer.Get<EntityModificatorsContainer>().AppliedModificators.Count < minimalModificators)
            {
                minimalModificators = buildingEntity.ComponentsContainer.Get<EntityModificatorsContainer>().AppliedModificators.Count;
                foundEntity = buildingEntity;
            }
        }
        
        return foundEntity;
    }
}