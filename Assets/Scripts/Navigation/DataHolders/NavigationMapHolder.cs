namespace Navigation
{
    public sealed class NavigationMapHolder
    {
        private NavigationMap _navigationMap;

        public NavigationMap Map => _navigationMap;

        public void SetNavigationMap(NavigationMap newMap) => _navigationMap = newMap;
    }
}