using UnityEngine.Events;
using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class EnemyBiome : MonoBehaviour
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;
        
        [Inject] private EnemyBiomeMeshGenerator _terrainMeshGenerator;
        [Inject] private TextureManager _textureManager;
        [Inject] private IslandGridHolder _islandGridHolder;
        [Inject] private EnemyBiomeMapGenerator _enemyBiomeMapGenerator;
        [Inject] private EnemyBiomeMapToGridConverter _enemyBiomeMapToGridConverter;
        [Inject] private OverlappingIslandDecorationsDisabler _overlappingIslandDecorationsDisabler;
        [Inject] private EnemySpawnerSystem _enemySpawnerSystem;
        [Inject] private SpawnerRotator _spawnerRotator;

        [SerializeField] private TerrainSetter _terrainSetter;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private EnemyBiomeDecorationGenerator _enemyBiomeDecorationGenerator;
        [SerializeField] private TerrainAnimator _terrainAnimator;
        [SerializeField] private EnemyBiomeDecorationMaterialChanger _enemyBiomeDecorationManager;

        private Vector2Int _centerPosition;
        private BlockGrid _currentBlockGrid;

        private Vector2Int _spawnerNodeIndex;
        public Vector2Int SpawnerNodeIndex => _spawnerNodeIndex;

        private int _currentStage;

        private void Awake()
        {
            _enemySpawnerSystem.AddSpawner(_enemySpawner);
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
        public int GetStage() => _currentStage;

        private void AdjustPosition()
        {
            int radius = _islandData.EnemyBiomeStages[_currentStage].EnemyBiomeRadius;

            transform.position = new Vector3(_centerPosition.x - radius, 0f, _centerPosition.y - radius);

            float height = _islandGridHolder.Grid.GetMaxHeight(_centerPosition.x, _centerPosition.y);

            if (height == 0) height += 1f;

            _enemySpawner.transform.localPosition = new Vector3(radius, height + 1f, radius);
        }

        public void RegenerateBiome()
        {
            AdjustPosition();

            bool[,] enemyBiomeMap = _enemyBiomeMapGenerator.GenerateEnemyMap(GetStage());

            _currentBlockGrid = _enemyBiomeMapToGridConverter.ConvertEnemyMapToGrid(enemyBiomeMap, GetStage(), GetBiomePosition());

            _overlappingIslandDecorationsDisabler.DisableOverlappingIslandDecorations(enemyBiomeMap, GetStage(), GetBiomePosition());

            GenerateMesh(_currentBlockGrid);

            _spawnerRotator.RotateSpawner(_enemySpawner.transform);
        }

        public void GenerateDecorations()
        {
            int radius = _islandData.EnemyBiomeStages[_currentStage].EnemyBiomeRadius;

            Vector2Int currentPos = new Vector2Int((int)(transform.position.x), (int)(transform.position.z));

            _enemyBiomeDecorationGenerator.SetDecorationModule(_islandData.EnemyBiomeStages[_currentStage].DecorationsModule);

            _enemyBiomeDecorationGenerator.GenerateDecorations(_currentBlockGrid, currentPos);
            
            _enemyBiomeDecorationManager.ApplyTransitionMaterial();
        }

        private void GenerateMesh(BlockGrid blockGrid)
        {
            _terrainMeshGenerator.SetupGenerator(blockGrid, _textureManager);
            _terrainMeshGenerator.SetPosition(new Vector3Int((int)(transform.position.x), 0, (int)(transform.position.z)));

            Mesh mesh = _terrainMeshGenerator.GetMesh();

            _terrainSetter.SetMesh(mesh);
        }

        public void Destroy()
        {
            _enemySpawnerSystem.RemoveSpawner(_enemySpawner);

            Destroy(gameObject);
        }   
    }
}