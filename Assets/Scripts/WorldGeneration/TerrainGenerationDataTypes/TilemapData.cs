using UnityEngine;

[CreateAssetMenu(fileName = "TilemapData", menuName = "Generation/TilemapData")]

public sealed class TilemapData : ScriptableObject
{
    [SerializeField] private MeshRenderer _defaultTile;
    [SerializeField] private MeshRenderer _fourSideTile;
    [SerializeField] private MeshRenderer _threeSideTile;
    [SerializeField] private MeshRenderer _twoSideTile;
    [SerializeField] private MeshRenderer _cornerTile;
    [SerializeField] private MeshRenderer _oneSideTile;
    [SerializeField] private MeshRenderer _topTile;

    public MeshRenderer DefaultTile => _defaultTile;
    public MeshRenderer FourSideTile => _fourSideTile;
    public MeshRenderer ThreeSideTile => _threeSideTile;
    public MeshRenderer TwoSideTile => _twoSideTile;
    public MeshRenderer CornerTile => _cornerTile;
    public MeshRenderer OneSideTile => _oneSideTile;
    public MeshRenderer TopTile => _topTile;
}