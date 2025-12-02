namespace WorldGeneration
{
    public sealed class RoadMapHolder
    {
        private bool[,] _roadMap;

        public bool[,] Map => _roadMap;

        public void SetRoadMap(bool[,] newMap) => _roadMap = newMap;
    }
}
