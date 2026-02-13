using WorldGeneration;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeData", menuName = "Generation/BiomeData")]

public sealed class BiomeData : ScriptableObject
{
    [SerializeField] private float _heightMultiplier;
    [SerializeField] private NoiseSetting[] _noiseSetting;
    [SerializeField] private DecorationModule _decorationModule;
    [SerializeField] private TilemapData _tilemapData;
    
    public float HeightMultiplier => _heightMultiplier;
    public NoiseSetting[] Noises => _noiseSetting;
    public DecorationModule DecorationsModule => _decorationModule;
    public TilemapData TilemapData => _tilemapData;
    
}