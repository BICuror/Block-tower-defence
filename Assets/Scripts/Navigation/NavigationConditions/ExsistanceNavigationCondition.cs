using UnityEngine;

namespace Navigation
{
    public sealed class ExsistanceNavigationCondition : INavigationCondition
    {
        public ExsistanceNavigationCondition(GameObject navigationObject)
        {
            _navigationObject = navigationObject;
        }

        private GameObject _navigationObject;

        public bool GetValue() => _navigationObject != null && _navigationObject.activeSelf;
    }
}