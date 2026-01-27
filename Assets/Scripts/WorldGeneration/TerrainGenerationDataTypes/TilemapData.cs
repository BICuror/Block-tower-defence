using UnityEngine;

[CreateAssetMenu(fileName = "TilemapData", menuName = "Generation/TilemapData")]

public sealed class TilemapData : ScriptableObject
{
    [SerializeField] private Tile _defaultTile;
    [SerializeField] private Tile _cornerTile;
    [SerializeField] private Tile _oneSideTile;
    [SerializeField] private Tile _defaultCornerTile;
    [SerializeField] private Tile _waterIndicatorTile;

    public Tile DefaultTile => _defaultTile;
    public Tile CornerTile => _cornerTile;
    public Tile OneSideTile => _oneSideTile;
    public Tile DefaultCornerTile => _defaultCornerTile;
    public Tile WaterIndicatorTile => _waterIndicatorTile;
}