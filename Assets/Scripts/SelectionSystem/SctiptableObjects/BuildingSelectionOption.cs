using UnityEngine;

[CreateAssetMenu(fileName = "BuildingSelectionOption", menuName = "Selection/BuildingSelectionOption")]

public sealed class BuildingSelectionOption : SelectionOption
{
    [SerializeField] private DraggableObject _building;
    public DraggableObject Building => _building;
}
