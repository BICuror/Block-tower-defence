using UnityEngine;

[CreateAssetMenu(fileName = "TilemapData", menuName = "Generation/TilemapData")]

public sealed class TilemapData : ScriptableObject
{
    [SerializeField] private GPUInstanceEnabler _defaultTile;
    [SerializeField] private GPUInstanceEnabler _fourSideTile;
    [SerializeField] private GPUInstanceEnabler _threeSideTile;
    [SerializeField] private GPUInstanceEnabler _twoSideTile;
    [SerializeField] private GPUInstanceEnabler _cornerTile;
    [SerializeField] private GPUInstanceEnabler _oneSideTile;
    [SerializeField] private GPUInstanceEnabler _topTile;

    public GPUInstanceEnabler DefaultTile => _defaultTile;
    public GPUInstanceEnabler FourSideTile => _fourSideTile;
    public GPUInstanceEnabler ThreeSideTile => _threeSideTile;
    public GPUInstanceEnabler TwoSideTile => _twoSideTile;
    public GPUInstanceEnabler CornerTile => _cornerTile;
    public GPUInstanceEnabler OneSideTile => _oneSideTile;
    public GPUInstanceEnabler TopTile => _topTile;
}