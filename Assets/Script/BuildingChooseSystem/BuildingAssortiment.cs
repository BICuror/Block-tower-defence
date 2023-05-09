using UnityEngine;

[CreateAssetMenu(fileName = "BuildingAssortiment", menuName = "BlockTowerDefence/BuildingAssortiment", order = 0)]

public class BuildingAssortiment : ScriptableObject 
{
    [SerializeField] private BuildingChoiseData[] _buildings;
    public BuildingChoiseData[] Buildings {get => _buildings;}
}
