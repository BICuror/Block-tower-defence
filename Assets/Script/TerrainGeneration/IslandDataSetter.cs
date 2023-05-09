using UnityEngine;

public sealed class IslandDataSetter : MonoBehaviour
{
    [SerializeField] private IslandData _islandDataToSet;
    
    private void Awake() => IslandDataContainer.SetIslandData(_islandDataToSet);
}
