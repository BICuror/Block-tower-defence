using Cysharp.Threading.Tasks;
using Zenject;

namespace Tutorial.Custom
{
    public class SelectionTutorialStep : UITutorialStep
    {
        [Inject] private GlobalBuildingContainer _globalBuildingContainer;
        [Inject] private SelectionManager _selectionManager;

        public override async UniTask StartStep()
        {
            await EnableUI();
            
            await _selectionManager.StartSelection(new SelectionSettings(SelectionType.BuildingUpgrade, 2), false);
            
            await UniTask.WaitWhile(() => _selectionManager.SelectionPhaseIsActive);
            
            CompleteStep();
        }

        public override async UniTask EndStep()
        {
            await DisableUI();
        }
    }
}