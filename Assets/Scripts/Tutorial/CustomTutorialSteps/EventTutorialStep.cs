using Cysharp.Threading.Tasks;
using Tutorial;
using Zenject;

namespace TutorialContent
{
    public sealed class EventTutorialStep : TutorialStep
    {
        [Inject] private GameController _gameController;
        
        public override UniTask StartStep()
        {
            CompleteStep();
            
            _gameController.DisableState(ControllerState.PositionDragging);
            _gameController.DisableState(ControllerState.Inspecting);
            _gameController.DisableState(ControllerState.Rotating);
            
            return UniTask.CompletedTask;
        }

        public override UniTask EndStep() => UniTask.CompletedTask;
    }
}