namespace Navigation
{
    public sealed class NavigationMap
    {
        public NavigationMap(int size)
        {
            _nodeMap = new NavigationNode[size, size];
        }

        private NavigationNode[,] _nodeMap;

        public void SetNode(int x, int y, NavigationNode node) => _nodeMap[x, y] = node;
        public NavigationNode GetNode(int x, int y) => _nodeMap[x, y];
    } 
}