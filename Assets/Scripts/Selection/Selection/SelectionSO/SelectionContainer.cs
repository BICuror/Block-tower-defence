using UnityEngine;

[CreateAssetMenu(fileName = "SelectionContainer", menuName = "Selection/SelectionContainer")]

public sealed class SelectionContainer : ScriptableObject 
{
    [SerializeField] private BuildingSelectionOptionDataContainer _buildingSelectionOptionDataContainer;    

    public BuildingSelectionOptionDataContainer BuildingSelectionOptionDataContainer => _buildingSelectionOptionDataContainer;
}