using UnityEngine;

[CreateAssetMenu(fileName = "BuildingChoiseData", menuName = "BlockTowerDefence/BuildingChoiseData", order = 0)]

public class BuildingChoiseData : ScriptableObject 
{
    [SerializeField] private Texture _buildingIcon;
    public Texture BuildingIcon {get => _buildingIcon;}

    [SerializeField] private GameObject _buildingPrefab;
    public GameObject BuildingPrefab {get => _buildingPrefab;}
}
