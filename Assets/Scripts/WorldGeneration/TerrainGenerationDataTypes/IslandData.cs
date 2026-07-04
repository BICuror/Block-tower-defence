using UnityEngine;
using UnityEngine.Serialization;

namespace WorldGeneration
{
    [CreateAssetMenu(fileName = "IslandData", menuName = "Generation/IslandData")]

    public sealed class IslandData : ScriptableObject
    {
        [Header("EniviromentSettings")]
        [SerializeField] private GameObject _eniviromentObject;
        public GameObject EniviromentObject => _eniviromentObject;

        [Header("GlobalStats")] 
        [SerializeField] private GameObject _contentControllerPrefab;
        public GameObject ContentControllerPrefab => _contentControllerPrefab;

        [SerializeField] private WavesContentConfig _wavesContentConfig;
        public WavesContentConfig WavesContentConfig => _wavesContentConfig;
        
        [SerializeField] private GlobalStatInitializerConfig _globalStatInitializerConfig;
        public GlobalStatInitializerConfig GlobalStatInitializerConfig => _globalStatInitializerConfig;

        [Header("SelectionSettings")] [Space] 
        [SerializeField] private EntityModificatorDataContainer _entityModificatorDataContainer;
        [SerializeField] private SelectionContainer _selectionContainer;
        
        public EntityModificatorDataContainer EntityModificatorDataContainer => _entityModificatorDataContainer;
        public SelectionContainer SelectionContainer => _selectionContainer;

        [Header("ItemSpawnSettings")][Space]
        [SerializeField] private ItemToggleEffectContainer _itemToggleEffectContainer;
        public ItemToggleEffectContainer ItemToggleEffectContainer => _itemToggleEffectContainer;

        [Header("EnemySpawnSettings")][Space]
        [SerializeField] private EnemyWavesData _enemyWavesData;
        public EnemyWavesData WavesData => _enemyWavesData;

        [Header("TextureSettings")][Space] 
        [SerializeField] private Texture _eniviromentTexture;
        public Texture EniviromentTexture => _eniviromentTexture;

        [SerializeField] private Texture _buildingsTexture;
        public Texture BuildingsTexture => _buildingsTexture;

        [SerializeField] private Texture _buildingsEmissionTexture;
        public Texture BuildingsEmissionTexture => _buildingsEmissionTexture;

        [SerializeField] private Texture _enemyBiomeDecorationsTextures;
        public Texture EnemyBiomeDecorationsTextures => _enemyBiomeDecorationsTextures;

        [SerializeField] private TilemapData _roadTilemap;
        public TilemapData RoadTilemap => _roadTilemap;
        
        [Header("RoadSettings")][Space] 
        [SerializeField] private SpawnerPositionValidator _spawnerPositionValidator;
        public SpawnerPositionValidator SpawnerPositionValidator => _spawnerPositionValidator;
        
        [SerializeField] private RoadGenerationAlgorithm _roadMapGenerationAlgorithm;
        public RoadGenerationAlgorithm RoadMapGenerationAlgorithm => _roadMapGenerationAlgorithm;

        [Header("RoadNodeSettings")][Space] 
        [SerializeField] private int _amountOfRoadNodesBetweenCenterAndEdge; 
        public int AmountOfRoadNodesBetweenCenterAndEdge => _amountOfRoadNodesBetweenCenterAndEdge;
        public int AmountOfRoadNodes => _amountOfRoadNodesBetweenCenterAndEdge * 2 + 3;
        public int CenterRoadNode => _amountOfRoadNodesBetweenCenterAndEdge + 2;

        [Header("GeneralIslandSettings")] [Space] 
        [Range(0f, 1f)] [SerializeField] private float _minimalSolidTilesPercent;
        public float MinimalSolidTilesPercent => _minimalSolidTilesPercent;
        [Range(0f, 1f)] [SerializeField] private float _maxSolidTilesPercent;
        public float MaxSolidTilesPercent => _maxSolidTilesPercent;
        [SerializeField] private int _islandRadius;
        public int IslandSize => _islandRadius * 2 + 1;
        public int IslandRadius => _islandRadius;
        public int CenterPositionIndex => _islandRadius;

        [SerializeField] private int _islandMaxHeight;
        public int IslandMaxHeight => _islandMaxHeight;

        [SerializeField] private int _islandHeightOffset;
        public int IslandHeightOffset => _islandHeightOffset;

        [Header("BiomesGenerationSettings")] [Space] 
        [SerializeField] private float _minimalBiomeIndexDistance = 2;
        [SerializeField] private BiomeSetting[] _biomes;
        
        public float MinimalBiomeIndexDistance => _minimalBiomeIndexDistance;
        public BiomeSetting[] Biomes => _biomes;

        public NoiseSetting[] BiomeGenerationNoises;
        public DecorationModule WaterDecorationsModule;

        [Header("EnemyBiomeStagesSettings")] [Space] 
        [SerializeField] private EnemyBiomeStage[] _enemyBiomeStages;
        public EnemyBiomeStage[] EnemyBiomeStages => _enemyBiomeStages;    
        
        [SerializeField] private int _corruptionLessZeroHeight;
        public int CorruptionLessZeroHeight => _corruptionLessZeroHeight;

        [SerializeField] private TilemapData _enemyBiomeTilemap;
        public TilemapData EnemyBiomeTilemap => _enemyBiomeTilemap;

        [System.Serializable] public struct EnemyBiomeStage
        {
            public DecorationModule DecorationsModule;

            [SerializeField] private int _enemyBiomeRadius;
            public int EnemyBiomeRadius => _enemyBiomeRadius;
            
            public AnimationCurve EnemyBiomeEdgeReductionCurve;
        }

        [Header("IslandShapeSettings")][Space] 
        [SerializeField] private SmoothingType _smoothingType;
        public SmoothingType IslandSmoothingType => _smoothingType;
        
        [Range(0f, 1f)]
        [SerializeField] private float _smoothingStrength;
        public float SmoothingStrength => _smoothingStrength;

        [Header("IslandBordersSettings")][Space] 

        [SerializeField] private bool _clearSingleEmptyBlocks;
        public bool ClearSingleEmptyBlocks => _clearSingleEmptyBlocks;

        [SerializeField] private bool _clearSingleSolidBlocks;
        public bool ClearSingleSolidBlocks => _clearSingleSolidBlocks;

        [SerializeField] private AnimationCurve _borderCurve;
        public AnimationCurve BorderCurve => _borderCurve;

        [Range(0f, 0.99f)]
        [SerializeField] private float _edgePrecantageCutout;
        public float EdgePrecantageCutout => _edgePrecantageCutout;

        [Range(0f, 1f)]
        [SerializeField] private float _edgeRandomAdditionalHeight;
        public float EdgeRandomAdditionalHeight => _edgeRandomAdditionalHeight;   
        
        public enum SmoothingType
        {
            None,
            CloseNeibours,
            AllNeibours
        }

        [Header("IslandCenterFlatSettings")][Space] 

        [SerializeField] private bool _centerShouldBeFlat;
        public bool CenterShouldBeFlat => _centerShouldBeFlat;

        [SerializeField] private int _flatRadius;
        public int FlatRadius => _flatRadius;

        [SerializeField] private int _flatHeightIncrease;
        public int FlatHeightIncrease => _flatHeightIncrease;
    }
    
    [System.Serializable] public struct BiomeSetting
    {
        [Range(0f, 1f)] [SerializeField] private float _appearRate;
        [SerializeField] private BiomeData _biomeData;
            
        public float AppearRate  => _appearRate;
        public BiomeData Data => _biomeData;
    }
    
    [System.Serializable] public struct NoiseSetting 
    {
        public AnimationCurve NoiseCurve;
        public Vector2 NoiseScale;
    }

    [System.Serializable] public struct DecorationModule
    {
        [Range(0f, 1f)] public float DecorationAppearRate;
        public Decoration[] Decorations; 

    }

    [System.Serializable] public struct Decoration
    {
        [Range(0f, 1f)] [SerializeField] private float _appearRate;
        public float AppearRate => _appearRate;

        public DecorationData DecorationData; 
    }
}