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
    [SerializeField] private TerrainAnimator _roadAnimator;
    
    [Inject] private CameraPositionController _cameraPositionController;
    
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private WaveIndexContainer _waveIndexContainer;
    
    [Inject] private IslandDecorationContainer _decorationContainer;

    [Inject] private EnemySpawnGroupCompiler _enemySpawnGroupCompiler;
    [Inject] private EnemyBiomeContainer _enemyBiomesContainer;
    [Inject] private EnemyBiomeGenerator _enemyBiomeGenerator;
    [Inject] private EnemySpawnSystem _enemySpawnSystem;
    
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
        
        _decorationContainer.UpdateDecorationsState();
        UpdateEnemyBiomesAmount();
        
        
        await _itemContainerManager.UpdateContainedItems();
        await TryStartBuildingSelection();
        
        GenerateEnemyGroups();
        
        await RegenerateRoads();
        
        RegenerateEnemyBiomes();
        
        _enemySpawnSystem.SetEnemyGroupVisibility(true);
        _enemyBiomesContainer.EnableBiomesTerrain(TransitionInDuration);
        _roadAnimator.StartAppearing(TransitionInDuration);
    }

    protected override async UniTask OnEnterStateCompleted()
    {
        RandomExstentions.ReInitializeUnityRandom();
        
        _cameraPositionController.SetDefaultPosition();
        
        _selectionManager.TryStartQueuedSelection();

        await UniTask.WaitWhile(() => _selectionManager.SelectionPhaseIsActive);

        _itemContainerManager.UnlockContainer();
    }

    protected override async UniTask OnQuitStateStarted()
    {
        _itemContainerManager.LockContainer();
        _itemFactory.DestroyAllUnusedItems();
        _enemySpawnSystem.SetEnemyGroupVisibility(false);
    }

    private void UpdateEnemyBiomesAmount()
    {
        RandomExstentions.ReInitializeUnityRandom();

        int biomesCount = _islandDataContainer.Data.WavesContentConfig.GetWaveContent(_waveIndexContainer.GetCurrentWave()).EnemySpawnersAmount;
        
        _enemyBiomesContainer.DestroyOldBiomes();
        _enemyBiomesContainer.DestroyExcessiveBiomes(biomesCount);
        _enemyBiomeGenerator.GenerateBiomes(biomesCount);
    }

    private void RegenerateEnemyBiomes()
    {
        RandomExstentions.ReInitializeUnityRandom();
        
        _enemyBiomesContainer.RegenerateBiomes();
        _enemyBiomesContainer.GenerateBiomesDecorations();
    }

    private async UniTask TryStartBuildingSelection()
    {
        if (_islandDataContainer.Data.WavesContentConfig.GetWaveContent(_waveIndexContainer.GetCurrentWave()).Content.Contains(WaveContentType.BuildingSelection))
        {
            _selectionManager.StartSelectionPhase();
            _selectionManager.StartSelection(SelectionType.Building, false).Forget();
            await UniTask.WaitWhile(() => _selectionManager.SelectionPhaseIsActive);
        }
    }

    private void GenerateEnemyGroups()
    {
        _enemySpawnGroupCompiler.RegenerateWaveSeed();
        _enemySpawnGroupCompiler.SetNextWaveData(_islandDataContainer.Data.WavesContentConfig.GetWaveContent(_waveIndexContainer.GetCurrentWave()).ForceExistingBuildingAttackWaves);
        _enemySpawnGroupCompiler.GenerateEnemyGroups();
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