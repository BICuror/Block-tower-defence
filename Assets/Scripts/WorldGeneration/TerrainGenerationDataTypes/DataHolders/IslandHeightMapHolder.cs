namespace WorldGeneration
{
    public sealed class IslandHeightMapHolder
    {
        private int[,] _heightMap;

        public int[,] Map => _heightMap;

        public int GetHeightSafe(int x, int z)
        {
            if (x < 0 || z < 0) return 0;
            if (x >= _heightMap.GetLength(0) || z >= _heightMap.GetLength(1)) return 0;

            return _heightMap[x, z];
        }
        
        public void SetMap(int[,] map) => _heightMap = map;
    }
}