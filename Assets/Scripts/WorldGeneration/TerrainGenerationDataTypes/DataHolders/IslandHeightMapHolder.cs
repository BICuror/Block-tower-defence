namespace WorldGeneration
{
    public sealed class IslandHeightMapHolder
    {
        private int[,] _heightMap;

        public int[,] Map => _heightMap;

        public int GetHeightSafe(int x, int y)
        {
            if (x < 0 || y < 0) return 0;
            if (x > _heightMap.GetLength(0) || y > _heightMap.GetLength(1)) return 0;

            return _heightMap[x, y];
        }
        
        public void SetMap(int[,] map) => _heightMap = map;
    }
}