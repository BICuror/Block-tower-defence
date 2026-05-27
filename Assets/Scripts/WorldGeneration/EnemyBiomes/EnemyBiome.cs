using UnityEngine;
using Zenject;
using Combat;

namespace WorldGeneration
{
    public sealed class EnemyBiome : MonoBehaviour
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;
        
        [Inject] private EnemyBiomeMeshGenerator _terrainMeshGenerator;
        [Inject] private IslandGridHolder _islandGridHolder;
        [Inject] private EnemyBiomeMapGenerator _enemyBiomeMapGenerator;
        [Inject] private EnemyBiomeMapToGridConverter _enemyBiomeMapToGridConverter;
        [Inject] private OverlappingIslandDecorationsDisabler _overlappingIslandDecorationsDisabler;
        [Inject] private EnemySpawnSystem _enemySpawnSystem;
        [Inject] private SpawnerRotator _spawnerRotator;

        [SerializeField] private EnemyBiomeTileTerrainGenerator _enemyBiomeTileTerrainGenerator;
        [SerializeField] private TerrainSetter _terrainSetter;
        [SerializeField] private TerrainSetter _bottomTerrainSetter;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private EnemyBiomeDecorationGenerator _enemyBiomeDecorationGenerator;
        [SerializeField] private TerrainAnimator _terrainAnimator;
        [SerializeField] private EnemyBiomeDecorationMaterialChanger _enemyBiomeDecorationManager;

        private Vector2Int _centerPosition;
        private BlockGrid _currentBlockGrid;
        private Vector2Int _spawnerNodeIndex;
        private int _currentStage;
        
        public Vector2Int SpawnerNodeIndex => _spawnerNodeIndex;
        public EnemySpawner EnemySpawner => _enemySpawner;
        public int CurrentStage => _currentStage;
        
        public void Initialize()
        {
            _enemySpawnSystem.AddSpawner(_enemySpawner);
        }

        public void SetSpawnerNodeIndex(Vector2Int spawnerNodeIndex) => _spawnerNodeIndex = spawnerNodeIndex;
        public void SetCenterPosition(Vector2Int position)
        {
            _centerPosition = position;
        
            _terrainAnimator.SetCenter(new Vector3(position.x, transform.position.y, position.y));

            AdjustPosition();
        }
        public Vector2Int GetCenterPosition() => _centerPosition;
        public Vector2Int GetBiomePosition() => new Vector2Int((int)transform.position.x, (int)transform.position.z);

        public void DisableTerrain(float duration) => _terrainAnimator.StartDisappearing(duration);
        public void EnableTerrain(float duration) => _terrainAnimator.StartAppearing(duration);

        public void IncreaseCurrentStage() => _currentStage++;

        private void AdjustPosition()
        {
            int radius = _islandData.EnemyBiomeStages[_currentStage].EnemyBiomeRadius;

            transform.position = new Vector3(_centerPosition.x - radius - 1f, 0f, _centerPosition.y - radius - 1f);

            float height = _islandGridHolder.Grid.GetMaxHeight(_centerPosition.x, _centerPosition.y);

            if (height == 0) height += 1f;

            _enemySpawner.transform.localPosition = new Vector3(radius + 1f, height + 1f, radius + 1f);
        }

        public void RegenerateBiome()
        {
            AdjustPosition();

            bool[,] enemyBiomeMap = _enemyBiomeMapGenerator.GenerateEnemyMap(_currentStage);

            _currentBlockGrid = _enemyBiomeMapToGridConverter.ConvertEnemyMapToGrid(enemyBiomeMap, _currentStage, GetBiomePosition());

            _overlappingIslandDecorationsDisabler.DisableOverlappingIslandDecorations(enemyBiomeMap, _currentStage, GetBiomePosition());

            GenerateMesh(_currentBlockGrid);
            
            _enemyBiomeTileTerrainGenerator.GenerateTerrain(enemyBiomeMap);
            
            _spawnerRotator.RotateSpawner(_enemySpawner.transform);
        }

        public void GenerateDecorations()
        {
            Vector2Int currentPos = new Vector2Int((int)(transform.position.x), (int)(transform.position.z));

            _enemyBiomeDecorationGenerator.SetDecorationModule(_islandData.EnemyBiomeStages[_currentStage].DecorationsModule);

            _enemyBiomeDecorationGenerator.GenerateDecorations(_currentBlockGrid, currentPos);
            
            _enemyBiomeDecorationManager.ApplyTransitionMaterial();
        }

        private void GenerateMesh(BlockGrid blockGrid)
        {
            _terrainMeshGenerator.SetupGenerator(blockGrid);
            _terrainMeshGenerator.SetPosition(new Vector3Int((int)(transform.position.x), 0, (int)(transform.position.z)));
            
            Mesh mesh = _terrainMeshGenerator.GetDefaultMesh();

            _terrainSetter.SetMesh(mesh);
            _bottomTerrainSetter.SetMesh(_terrainMeshGenerator.GetBottomMesh());
        }

        public void Destroy()
        {
            _enemySpawnSystem.RemoveSpawner(_enemySpawner);

            Destroy(gameObject);
        }   
    }
}