namespace WorldGeneration
{
    public sealed class IslandHeightMapHolder
    {
        private int[,] _heightMap;

        public int[,] Map => _heightMap;
        
        public void SetMap(int[,] map) => _heightMap = map;
    }
}