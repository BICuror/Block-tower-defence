using UnityEngine;

public sealed class BlockGrid
{
    private BlockType[,,] _islandGrid; 

    private int _height;
    private int _size;

    public BlockGrid(int size, int height)
    {
        _height = height;
        _size = size;

        _islandGrid = new BlockType[_size, _height, _size];
    }

    public int GetHeight() => _height;
    public int GetSize() => _size;

    public void SetBlockType(Vector3Int position, BlockType type)
    {
        _islandGrid[position.x, position.y, position.z] = type;
    }

    public bool GridSpaceIsEmpty(Vector3Int position)
    {
        return _islandGrid[position.x, position.y, position.z] == BlockType.Empty;
    }

    public int GetMaxHeight(int x, int z)
    {
        for (int height = _height - 1; height > 0; height--)
        {
            if (_islandGrid[x, height, z] != BlockType.Empty) return height;
        }

        return 0;
    }

    public bool IsInBounds(Vector3Int position)
    {
        if (position.x < 0 || position.x > _size - 1) return false;
        if (position.y < 0 || position.y > _height - 1) return false;
        if (position.z < 0 || position.z > _size - 1) return false;

        return true;
    }

    public BlockType GetBlockType(Vector3Int position)
    {
        return _islandGrid[position.x, position.y, position.z];
    }
}
