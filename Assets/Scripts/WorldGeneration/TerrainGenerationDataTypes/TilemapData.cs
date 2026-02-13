using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "TilemapData", menuName = "Generation/TilemapData")]

public sealed class TilemapData : ScriptableObject
{
    [SerializeField] private Tile _defaultTile;
    [SerializeField] private Tile _cornerTile;
    [SerializeField] private Tile _oneSideTile;
    [SerializeField] private Tile _defaultCornerTile;
    [SerializeField] private Tile _waterIndicatorTile;
    
    [Header("Seams")]
    [SerializeField] private bool _hasSeams;
    [ShowIf("_hasSeams")] [SerializeField] private int _seamPriority;
    [ShowIf("_hasSeams")] [SerializeField] private Tile _seamTile;

    public Tile DefaultTile => _defaultTile;
    public Tile CornerTile => _cornerTile;
    public Tile OneSideTile => _oneSideTile;
    public Tile DefaultCornerTile => _defaultCornerTile;
    public Tile WaterIndicatorTile => _waterIndicatorTile;
    
    public int SeamPriority => _seamPriority;
    public Tile SeamTile => _seamTile;
}