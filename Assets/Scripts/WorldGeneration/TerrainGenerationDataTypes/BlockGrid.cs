using UnityEngine;

namespace WorldGeneration
{
    public class BlockGrid
    {
        private bool[,,] _grid; 

        private int _height;
        private int _size;

        public int GetHeight() => _height;
        public int GetSize() => _size;
        
        public BlockGrid(int size, int height)
        {
            _size = size;
            _height = height;
            
            _grid = new bool[_size, _height, _size];
        }

        public void SetBlock(Vector3Int position)
        {
            _grid[position.x, position.y, position.z] = true;
        }

        public bool GridSpaceIsEmpty(Vector3Int position)
        {
            return !_grid[position.x, position.y, position.z];
        }

        public int GetMaxHeight(int x, int z)
        {
            for (int height = 1; height < _height; height++)
            {
                if (!_grid[x, height, z]) return height - 1;
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
    }
}