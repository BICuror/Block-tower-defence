public static class IslandDataContainer
{
    private static IslandData s_islandData;
    
    public static void SetIslandData(IslandData islandData) => s_islandData = islandData;

    public static IslandData GetData() => s_islandData;
}
