using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine;

namespace UI
{
    public sealed class RebindScreen : MonoBehaviour
    {
        [SerializeField] private RebindSaveLoad _rebindSaveLoad;
    
        private void OnDisable()
        {
            _rebindSaveLoad.Save();
        }
    }
}