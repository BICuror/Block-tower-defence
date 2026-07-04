using Cysharp.Threading.Tasks;
using WorldGeneration;
using UnityEngine;
using Zenject;

public sealed class EntryPoint : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private WaveStateMachine _waveManager;
    [Inject] private DiContainer _diContainer;
    
    [SerializeField] private SceneStartAnimationController _sceneStartAnimationController;
    [SerializeField] private IslandGenerator _islandGenerator;

    private async void Awake()
    {
        Application.runInBackground = true;
        
        _islandGenerator.GenerateIsland();

        await UniTask.DelayFrame(1);

        await _sceneStartAnimationController.PlayAnimation();
        
        _waveManager.Initialize();
        
        _diContainer.InstantiatePrefab(_islandDataContainer.Data.ContentControllerPrefab); 
        
        _waveManager.TransitionIntoIdle();
    }
}
