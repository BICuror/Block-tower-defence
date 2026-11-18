using Combat.IngameUI.Inspection.InspectionCanvas;
using Combat.IngameUI.Inspection.Tooltips;
using Combat.IngameUI.Inspection;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameControls.States;
using GameControls;
using Zenject;

namespace Tutorial.Custom
{
    public sealed class BuildingInspectionTutorialStep : ProgressTutorialStep
    {
        [Inject] private GameController _gameController;
        
        private List<TooltipTagData> _inspectedTagDatas = new();
        
        public override async UniTask StartStep()
        {
            InspectionCanvasManager.Instance.OpenedTooltip += TrySubscribeToEntityTooltipSync;
            
            await EnableUI();
            
            _gameController.EnableState(ControllerState.Inspecting);
        }

        private void TrySubscribeToEntityTooltipSync(InspectionCanvasBase inspectionCanvasBase) => TrySubscribeToEntityTooltip(inspectionCanvasBase).Forget();
        private async UniTask TrySubscribeToEntityTooltip(InspectionCanvasBase inspectionCanvasBase)
        {
            if (inspectionCanvasBase is not EntityInspectionCanvas) return;

            TooltipController tooltipController = inspectionCanvasBase.GetComponentInChildren<TooltipController>();

            tooltipController.CreatedTooltip += TryIncreaseProgress;
            
            DisableUI();
            
            await UniTask.WaitWhile(() => inspectionCanvasBase);
            
            if (IsComplete) return;

            EnableUI();
        }

        private void TryIncreaseProgress(TooltipTagData tagData)
        {
            if (_inspectedTagDatas.Contains(tagData)) return;
            
            _inspectedTagDatas.Add(tagData);
            
            IncreaseProgress();
        }

        public override async UniTask EndStep()
        {
            InspectionCanvasManager.Instance.OpenedTooltip -= TrySubscribeToEntityTooltipSync;
            
            await DisableUI();
        }
    }
}