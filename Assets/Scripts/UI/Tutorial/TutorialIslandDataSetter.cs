using UnityEngine;
using WorldGeneration;
using Zenject;

public sealed class TutorialIslandDataSetter : MonoBehaviour
{
    [SerializeField] private IslandData _tutorialIslandData;
    [Inject] private IslandDataContainer _islandDataContainer;
    private IslandData _savedIslandData;

    private void Awake()
    {
        _savedIslandData = _islandDataContainer.Data;

        _islandDataContainer.SetData(_tutorialIslandData);
    }

    public void OnDestroy() 
    {
        _islandDataContainer.SetData(_savedIslandData);    
    }
}
