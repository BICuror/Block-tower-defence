namespace Navigation
{
    public sealed class NavigationMapHolder
    {
        private NavigationMap _navigationMap = new();
        public NavigationMap Map => _navigationMap;
    }
}