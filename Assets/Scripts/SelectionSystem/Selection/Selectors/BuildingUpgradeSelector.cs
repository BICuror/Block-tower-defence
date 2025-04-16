using System.Collections.Generic;
using UnityEngine;
using Combat;

public sealed class BuildingUpgradeSelector : MonoBehaviour
{
    [SerializeField] private BuildingEntity _buildingEntityToUpgrade;
    [SerializeField] private BuildingUpgradeSelectionOptionObject _buildingUpgradeSelectionOptionObjectPrefab;
    [SerializeField] private SelectionOptionObjectController _selectionOptionObjectController;
    [SerializeField] private int _optionsAmount = 3;
    [SerializeField] private Transform _centerPosition;
    private Vector3 _initialPosition;

    public async void StartGlobalEffectSelection()
    {
        _initialPosition = _buildingEntityToUpgrade.transform.position;
        _buildingEntityToUpgrade.transform.position = _centerPosition.position;
        _buildingEntityToUpgrade.ComponentsContainer.Get<DraggableObject>().SetDraggableState(false);
        
        List<EntityModificatorData> effectDatas = GetRandomEntityEffectDatas(_buildingEntityToUpgrade, _optionsAmount);

        for (int i = 0; i < effectDatas.Count; i++)
        {
            BuildingUpgradeSelectionOptionObject selectionOptionObject = await _selectionOptionObjectController.CreateSelectionOptionObject(_buildingUpgradeSelectionOptionObjectPrefab);
            selectionOptionObject.SetTargetBuildingEntity(_buildingEntityToUpgrade);
            selectionOptionObject.SetEffectData(effectDatas[i]);
        }
    }

    public void EndSelection()
    {
        _buildingEntityToUpgrade.transform.position = _initialPosition;
        _buildingEntityToUpgrade.ComponentsContainer.Get<DraggableObject>().SetDraggableState(true);
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
}