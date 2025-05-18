public sealed class GlobalStatContainer : StatContainer
{
    public GlobalStatContainer(IslandDataContainer islandDataContainer)
    {
        AddStats(islandDataContainer.Data.GlobalStatInitializerConfig.StatInitializers);
    }
}