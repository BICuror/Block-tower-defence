using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Combat;

public sealed class BuildingUpgradeSelector : MonoBehaviour
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [SerializeField] private DraggableConnector _draggableConnector;
    [SerializeField] private BuildingUpgradeSelectionOptionObject _buildingUpgradeSelectionOptionObjectPrefab;
    [SerializeField] private SelectionOptionObjectController _selectionOptionObjectController;
    [SerializeField] private Transform _centerPosition;
    private BuildingEntity _buildingEntityToUpgrade;
    private Vector3Int _initialPosition;

    private void Awake()
    {
        _draggableConnector.transform.SetParent(null);
    }
    
    public async UniTask StartUpgradeSelection(SelectionSettings settings)
    {
        if (settings.SelectionArgument != null)
        {
            _buildingEntityToUpgrade = (BuildingEntity)settings.SelectionArgument;
        }
        else
        {
            _buildingEntityToUpgrade = _globalBuildingContainer.Entities[Random.Range(0, _globalBuildingContainer.Entities.Count)];
        }
            
        _initialPosition = Vector3Int.RoundToInt(_buildingEntityToUpgrade.transform.position);
        
        await CaptureDraggable();
        
        List<EntityModificatorData> effectDatas = GetRandomEntityEffectDatas(_buildingEntityToUpgrade, 3);

        for (int i = 0; i < effectDatas.Count; i++)
        {
            BuildingUpgradeSelectionOptionObject selectionOptionObject = await _selectionOptionObjectController.CreateSelectionOptionObject(_buildingUpgradeSelectionOptionObjectPrefab);
            selectionOptionObject.SetTargetBuildingEntity(_buildingEntityToUpgrade);
            selectionOptionObject.SetEffectData(effectDatas[i]);
        }
    }

    public async UniTask EndSelection()
    {
        await ReleaseDraggable();
    }

    private List<EntityModificatorData> GetRandomEntityEffectDatas(BuildingEntity entity, int amount)
    {
        List<EntityModificatorData> resultEffectDatas = new();
        List<EntityModificatorData> allEffectDatas = entity.ComponentsContainer.Get<EntityModificatorsContainer>().AvailableModificators;

        for (int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, allEffectDatas.Count);
            resultEffectDatas.Add(allEffectDatas[randomIndex]);
            allEffectDatas.RemoveAt(randomIndex);
        }

        return resultEffectDatas;
    }
    
    private async UniTask CaptureDraggable()
    {
        _draggableConnector.gameObject.SetActive(true);
        _draggableConnector.transform.position = _centerPosition.position;

        await _draggableConnector.MoveTo(_buildingEntityToUpgrade.transform.position, 0.2f);
        
        _draggableConnector.PickUpDraggable(_buildingEntityToUpgrade.gameObject);

        await _draggableConnector.MoveTo(_centerPosition.transform.position, 0.2f);
    }

    private async UniTask ReleaseDraggable()
    {
        _draggableConnector.transform.position = _centerPosition.position;

        Vector3 placementPosition = TileMap.GetNearestPlacePosition(_buildingEntityToUpgrade.ComponentsContainer.Get<BuildingDraggable>(), _initialPosition);

        await _draggableConnector.MoveTo(placementPosition, 0.2f);

        await _draggableConnector.PlaceDraggable(_buildingEntityToUpgrade.gameObject, _buildingEntityToUpgrade.ComponentsContainer.Get<BuildingDraggable>(), placementPosition);

        _draggableConnector.gameObject.SetActive(false);
    }
}