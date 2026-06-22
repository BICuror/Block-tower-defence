using Cysharp.Threading.Tasks;
using GameControls.States;
using GameControls;
using Zenject;

namespace Tutorial.Custom
{
    public sealed class TutorialInitializationStep : TutorialStep
    {
        [Inject] private GameController _gameController;
        
        public override UniTask StartStep()
        {
            _gameController.DisableState(ControllerState.CameraRepositionDrag);
            _gameController.DisableState(ControllerState.Inspecting);
            _gameController.DisableState(ControllerState.Rotating);
            
            CompleteStep();
            
            return UniTask.CompletedTask;
        }

        public override UniTask EndStep() => UniTask.CompletedTask;
    }
}