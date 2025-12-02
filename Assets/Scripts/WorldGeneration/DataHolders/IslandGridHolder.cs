namespace WorldGeneration
{
    public sealed class IslandGridHolder
    {
        private BlockGrid _grid;

        public BlockGrid Grid => _grid;

        public void SetGrid(BlockGrid grid) => _grid = grid;
    }
}