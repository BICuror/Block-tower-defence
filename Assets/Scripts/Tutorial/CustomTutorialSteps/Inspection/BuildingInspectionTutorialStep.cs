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
            InspectionTooltipManager.Instance.OpenedTooltip += TrySubscribeToEntityTooltipSync;
            
            await EnableUI();
            
            _gameController.EnableState(ControllerState.Inspecting);
        }

        private void TrySubscribeToEntityTooltipSync(InspectionPanelBase inspectionPanelBase) => TrySubscribeToEntityTooltip(inspectionPanelBase).Forget();
        private async UniTask TrySubscribeToEntityTooltip(InspectionPanelBase inspectionPanelBase)
        {
            if (inspectionPanelBase is not EntityTooltip) return;

            InspectionTooltipController tooltipController = inspectionPanelBase.GetComponentInChildren<InspectionTooltipController>();

            tooltipController.CreatedTooltip += TryIncreaseProgress;
            
            DisableUI();
            
            await UniTask.WaitWhile(() => inspectionPanelBase);
            
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
            InspectionTooltipManager.Instance.OpenedTooltip -= TrySubscribeToEntityTooltipSync;
            
            await DisableUI();
        }
    }
}