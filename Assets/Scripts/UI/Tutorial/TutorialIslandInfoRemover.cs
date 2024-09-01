using Zenject;
using UnityEngine;
using WorldGeneration;

public sealed class TutorialIslandInfoRemover : MonoBehaviour
{
    [SerializeField] private IslandData _firstIslandData;
    [SerializeField] private IslandData _tutorialIslandData;
    [Inject] private IslandDataContainer _islandDataContainer;

    private void Awake()
    {
        if (_tutorialIslandData == _islandDataContainer.Data)
        {
            _islandDataContainer.SetData(_firstIslandData);
        }
    }

    public void SetTutorialIslandData() => _islandDataContainer.SetData(_firstIslandData);
}
