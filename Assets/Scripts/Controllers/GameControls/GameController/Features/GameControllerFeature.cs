using UnityEngine.InputSystem;
using UnityEngine;

namespace GameControls.Features
{
    public abstract class GameControllerFeature : MonoBehaviour
    {
        protected InputActionMap InputActionMap;
        
        public void SetInputActionMap(InputActionMap actionMap) => InputActionMap = actionMap;
        public abstract void Initialize();

        public void EnableFeature()
        {
            enabled = true;
            OnEnableFeature();
        }

        public void DisableFeature()
        {
            enabled = false;
            OnDisableFeature();
        }
        
        protected abstract void OnEnableFeature();
        protected abstract void OnDisableFeature();
    }
}