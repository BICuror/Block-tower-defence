using UnityEngine;

[CreateAssetMenu(fileName = "TilemapData", menuName = "Generation/TilemapData")]

public sealed class TilemapData : ScriptableObject
{
    [SerializeField] private GPUInstanceEnabler _defaultTile;
    [SerializeField] private GPUInstanceEnabler _cornerTile;
    [SerializeField] private GPUInstanceEnabler _oneSideTile;
    [SerializeField] private GPUInstanceEnabler _defaultCornerTile;

    public GPUInstanceEnabler DefaultTile => _defaultTile;
    public GPUInstanceEnabler CornerTile => _cornerTile;
    public GPUInstanceEnabler OneSideTile => _oneSideTile;
    public GPUInstanceEnabler DefaultCornerTile => _defaultCornerTile;
}