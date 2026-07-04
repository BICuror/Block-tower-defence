using Cysharp.Threading.Tasks;
using GameControls.Features;
using GameControls;
using Zenject;

namespace Tutorial.Custom
{
    public sealed class ChangeScaleTutorialStep : ProgressTutorialStep
    {
        [Inject] private CameraZoomGameControllerFeature _cameraZoomGameControllerFeature;
        [Inject] private GameController _gameController;

        public override async UniTask StartStep()
        {
            await EnableUI();
            
            _gameController.EnableFeature(ControllerFeature.CameraZoom);
            
            _cameraZoomGameControllerFeature.ZoomChanged += IncreaseProgress;
        }

        public override async UniTask EndStep()
        {
            _cameraZoomGameControllerFeature.ZoomChanged -= IncreaseProgress;
            await DisableUI();
        }
    }
}