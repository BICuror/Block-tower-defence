using UnityEngine;
using Combat;

[CreateAssetMenu(fileName = "BuildingSelectionOptionData",
    menuName = "Selection/OptionDatas/BuildingSelectionOptionData")]

public sealed class BuildingSelectionOptionData : SelectionOptionData
{
    [Header("BuildingSelectionSettings")] 
    [SerializeField] private BuildingEntity _buildingEntity;

    public BuildingEntity BuildingPrefab => _buildingEntity;
}