using GameControls.Controllers;
using Cysharp.Threading.Tasks;
using GameControls.Features;
using GameControls.States;
using GameControls;
using Zenject;

namespace Tutorial.Custom
{
    public sealed class MoveCameraTutorialStep : ProgressTutorialStep
    {
        [Inject] private CameraController _cameraController;
        [Inject] private GameController _gameController;
        
        public override async UniTask StartStep()
        {
            await EnableUI();
            
            _gameController.EnableState(ControllerState.CameraRepositionDrag);
            _gameController.EnableFeature(ControllerFeature.CameraRepositioning);
            
            _cameraController.CameraPositionUpdated += IncreaseProgress;
        }

        public override async UniTask EndStep()
        {
            _cameraController.CameraPositionUpdated -= IncreaseProgress;
            await DisableUI();
        }
    }
}