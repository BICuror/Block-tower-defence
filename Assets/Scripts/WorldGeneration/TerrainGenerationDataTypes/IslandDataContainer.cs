using UnityEngine;
using WorldGeneration;

[CreateAssetMenu(fileName = "IslandDataContainer", menuName = "Installers/IslandDataContainer")]

public class IslandDataContainer : ScriptableObject 
{
    [SerializeField] private IslandData _islandData;

    public IslandData Data => _islandData;

    public void SetData(IslandData data) => _islandData = data;    
}
