using UnityEngine.InputSystem;
using Zenject;

namespace GameControls.Features
{
    public sealed class TimeGameControllerFeature : GameControllerFeature
    {
        [Inject] private TimeController _timeController;
        
        private const float DEFAULT_TIME_SCALE = 1f;

        private InputAction _toggleTimeAction;
        private bool _speedUpTime;

        public override void Initialize()
        {
            _toggleTimeAction = InputActionMap["ToggleTime"];
        }

        protected override void OnEnableFeature()
        {
            _toggleTimeAction.performed += ToggleTimeScale;
        }

        protected override void OnDisableFeature()
        {
            _toggleTimeAction.performed -= ToggleTimeScale;
        }
        
        private void ToggleTimeScale(InputAction.CallbackContext _) => _timeController.ToggleTimeScale();
    }
}