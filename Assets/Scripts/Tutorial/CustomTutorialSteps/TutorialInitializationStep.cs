using Cysharp.Threading.Tasks;
using GameControls.Features;
using GameControls.States;
using GameControls;
using Zenject;

namespace Tutorial.Custom
{
    public sealed class TutorialInitializationStep : TutorialStep
    {
        [Inject] private GameController _gameController;

        private void Start() => DisableControllerFeaturesAndStates();
        
        public override UniTask StartStep()
        {
            DisableControllerFeaturesAndStates();
            
            CompleteStep();
            
            return UniTask.CompletedTask;
        }

        private void DisableControllerFeaturesAndStates()
        {
            _gameController.DisableState(ControllerState.CameraRepositionDrag);
            _gameController.DisableState(ControllerState.Inspecting);
            _gameController.DisableState(ControllerState.Rotating);
            _gameController.DisableState(ControllerState.Dragging);
            
            _gameController.DisableFeature(ControllerFeature.CameraRepositioning);
            _gameController.DisableFeature(ControllerFeature.HoverableFeature);
            _gameController.DisableFeature(ControllerFeature.CameraZoom);
            _gameController.DisableFeature(ControllerFeature.TimeToggle);
        }
        
        public override UniTask EndStep() => UniTask.CompletedTask;
    }
}