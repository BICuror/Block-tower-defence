public sealed class RoadWeightMapHolder
{
    private int[,] _weightMap;

    public int[,] Map => _weightMap;

    public void SetWeightMap(int[,] newMap) => _weightMap = newMap;
}