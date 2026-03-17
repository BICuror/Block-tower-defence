using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using WorldGeneration;
using UnityEngine;
using System.Linq;
using Combat;
using Navigation;
using Zenject;

public sealed class IdleStateController : WaveStateController
{
    [SerializeField] private CameraPositionController _cameraPositionController;
    [SerializeField] private TerrainAnimator _roadAnimator;
    
    [Inject] private WaveIndexContainer _waveIndexContainer;
    
    [Inject] private IslandDecorationContainer _decorationContainer;
    
    [Inject] private EnemySpawnGroupCompiler _enemySpawnGroupCompiler;
    [Inject] private EnemyBiomeContainer _enemyBiomesContainer;
    [Inject] private EnemyBiomeGenerator _enemyBiomeGenerator;
    
    [Inject] private ItemContainerManager _itemContainerManager;
    [Inject] private SelectionManager _selectionManager;
    [Inject] private ItemFactory _itemFactory;
    
    [Inject] private RoadWeightMapGenerator _roadWeightMapGenerator;
    [Inject] private NavigationMapGenerator _navigationMapGenerator;
    [Inject] private OptionalTaskManager _optionalTaskManager; 
    [Inject] private RoadMapGenerator _roadMapGenerator;
    [Inject] private RoadGenerator _roadGenerator;

    public override WaveState GetControlledState() => WaveState.Idle;

    protected override async UniTask OnEnterStateStarted()
    {
        _waveIndexContainer.IncreaseWaveCounter();

        _decorationContainer.ActivateAllDecorations();
        _enemyBiomesContainer.DestroyOldBiomes();

        TryGenerateNewEnemyBiome();

        _enemySpawnGroupCompiler.GenerateWaveSeed();
        
        await RegenerateRoads();
        
        RegenerateEnemyBiomes();
        
        _enemyBiomesContainer.EnableBiomesTerrain(TransitionInDuration);
        _roadAnimator.StartAppearing(TransitionInDuration);
    }

    protected override async UniTask OnEnterStateCompleted()
    {
        RandomExstentions.ReInitializeUnityRandom();
        
        await _itemContainerManager.UpdateContainedItems();
        
        _cameraPositionController.SetDefaultPosition();
        
        _selectionManager.TryStartQueuedSelection();
        
        _itemContainerManager.UnlockContainer();
    }

    protected override async UniTask OnQuitStateStarted()
    {
        _itemContainerManager.LockContainer();
        _itemFactory.DestroyAllUnusedItems();
    }

    private void TryGenerateNewEnemyBiome()
    {
        RandomExstentions.ReInitializeUnityRandom();
        
        _enemyBiomeGenerator.TryGenerateNewBiome();
    }

    private void RegenerateEnemyBiomes()
    {
        RandomExstentions.ReInitializeUnityRandom();
        
        _enemyBiomesContainer.RegenerateBiomes();
        _enemyBiomesContainer.GenerateBiomesDecorations();
    }
    
    private async UniTask RegenerateRoads()
    {
        RandomExstentions.ReInitializeUnityRandom();

        await _roadMapGenerator.GenerateRoads();
        _roadWeightMapGenerator.GenerateRoadWeightMap();
        
        _optionalTaskManager.GenerateTasksAndModifyRoadMap();
        
        _navigationMapGenerator.GenerateMap();
        _roadGenerator.GenerateRoads();
    }

    [Button("RegenerateRoads")]
    public void RegenerateRoadsButton()
    {
        FindObjectsByType<OptionalTask>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).ToList().ForEach(bloodCollector => bloodCollector.gameObject.GetComponent<CombatEntity>().Health.Die());
        RegenerateRoads().Forget();
    }
}