using GameControls.Controllers;
using Cysharp.Threading.Tasks;
using GameControls.States;
using GameControls;
using Zenject;

namespace Tutorial.Custom
{
    public sealed class RotateCameraTutorialStep : ProgressTutorialStep
    {
        [Inject] private CameraController _cameraController;
        [Inject] private GameController _gameController;
        
        public override async UniTask StartStep()
        {
            await EnableUI();
            
            _gameController.EnableState(ControllerState.Rotating);
            
            _cameraController.CameraRotated += IncreaseProgress;
        }

        public override async UniTask EndStep()
        {
            _cameraController.CameraRotated -= IncreaseProgress;
            await DisableUI();
        }
    }
}