using UnityEngine;

namespace Navigation
{
    public sealed class NavigationMapHolder : MonoBehaviour
    {
        public static NavigationMapHolder Instance;
        
        private NavigationMap _navigationMap = new();
        public NavigationMap Map => _navigationMap;

        private void Awake() => Instance = this;
    }
}