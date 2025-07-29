namespace WorldGeneration
{
    public sealed class IslandTerrainMeshGenerator : MeshGenerator
    {
        protected override bool ShouldCheckThisBlockType(BlockType type)
        {
            return (type == BlockType.Empty || type == BlockType.Surface || type == BlockType.Rock || type == BlockType.Corruption);
        }
    }
}