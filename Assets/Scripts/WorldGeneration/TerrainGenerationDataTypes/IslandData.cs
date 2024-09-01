using UnityEngine;

namespace WorldGeneration
{
    [CreateAssetMenu(fileName = "IslandData", menuName = "Generation/IslandData")]

    public sealed class IslandData : ScriptableObject
    {
        #region Saving

        public void ReachedWave(int wave)
        {
            if (PlayerPrefs.GetInt(IslandName) < wave)
            {
                PlayerPrefs.SetInt(IslandName, wave); 
            }
        }

        public bool HasReachedWave(int wave) => wave <= PlayerPrefs.GetInt(IslandName);

        #endregion


        [SerializeField] private string _islandName;
        public string IslandName => _islandName;

        [SerializeField] private int _maxWave;
        public int MaxWave => _maxWave;

        [Header("EniviromentSettings")][Space]
        [SerializeField] private GameObject _eniviromentObject;
        public GameObject EniviromentObject => _eniviromentObject; 

        [Header("CrystalSpawnSettings")][Space]
        [SerializeField] private CrystalSpawnSettings _crystalSpawnSettings;
        public CrystalSpawnSettings CrystalSettings => _crystalSpawnSettings;

        [SerializeField] private SelectionOptionContainer _buildingsSelectionOptionContainer;
        public SelectionOptionContainer BuildingsSelectionOptionContainer => _buildingsSelectionOptionContainer;

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

        [Space]
        [SerializeField] private CubeTextures _roadBlock;
        public CubeTextures RoadBlock => _roadBlock;

        [SerializeField] private CubeTextures _roadBlockOnWater;
        public CubeTextures RoadBlockOnWater => _roadBlockOnWater;

        [SerializeField] private CubeTextures _corruptionBlock;
        public CubeTextures CorruptionBlock => _corruptionBlock;   

        [SerializeField] private CubeTextures _corruptionBlockOnWater;
        public CubeTextures CorruptionBlockOnWater => _corruptionBlockOnWater;    

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

        [Header("GeneralIslandSettings")][Space] 
        [SerializeField] private int _islandRadius;
        public int IslandSize => _islandRadius * 2 + 1;

        public int MiddleIndex => _islandRadius;

        [SerializeField] private int _islandMaxHeight;
        public int IslandMaxHeight => _islandMaxHeight;

        [SerializeField] private int _islandHeightOffset;
        public int IslandHeightOffset => _islandHeightOffset;
        
        [System.Serializable] public struct NoiseSetting 
        {
            public AnimationCurve NoiseCurve;
            public Vector2 NoiseScale;
        }
        
        [System.Serializable] public struct Biome
        {
            [Range(0f, 1f)] public float AppearRate;
            public string BiomeName;
            public CubeTextures SurfaceBiomBlock;
            public CubeTextures RockBiomeBlock;
            public CubeTextures BedrockBiomBlock;

            public float HeightMultiplier;

            public NoiseSetting[] Noises;

            public DecorationModule DecorationsModule;
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
        
        [Header("BiomesGenerationSettings")][Space] 
        
        [SerializeField] private Biome[] _biomes;
        public Biome[] Biomes => _biomes;

        public NoiseSetting[] BiomeGenerationNoises;

        [Header("EnemyBiomeStagesSettings")][Space] 

        [SerializeField] private int _maxAmountOfEnemyBiomes;
        public int MaxAmountOfEnemyBiomes => _maxAmountOfEnemyBiomes;

        [Range(1, 10)] [SerializeField] private int _begginingAmountOfEnemyBiomes;
        public int BegginingAmountOfEnemyBiomes => _begginingAmountOfEnemyBiomes;

        [SerializeField] private EnemyBiomeStage[] _enemyBiomeStages;
        public EnemyBiomeStage[] EnemyBiomeStages => _enemyBiomeStages;    
        
        [SerializeField] private int _corruptionLessZeroHeight;
        public int CorruptionLessZeroHeight => _corruptionLessZeroHeight;


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
}