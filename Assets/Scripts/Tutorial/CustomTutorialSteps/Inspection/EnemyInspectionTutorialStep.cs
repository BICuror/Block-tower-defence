using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Combat;

namespace Tutorial.Custom
{
    public sealed class EnemyInspectionTutorialStep : ProgressTutorialStep
    {
        private List<EnemyData> _inspectedEnemyData = new();
        
        public override async UniTask StartStep()
        {
            InspectionTooltipManager.Instance.OpenedTooltip += TrySubscribeToEntityTooltipSync;
            
            await EnableUI();
        }

        private void TrySubscribeToEntityTooltipSync(InspectionPanelBase inspectionPanelBase) => TrySubscribeToEntityTooltip(inspectionPanelBase).Forget();
        private async UniTask TrySubscribeToEntityTooltip(InspectionPanelBase inspectionPanelBase)
        {
            if (inspectionPanelBase is not EntityTooltip) return;

            EntityTooltip entityTooltip = inspectionPanelBase as EntityTooltip;

            if (entityTooltip.InspectedEntity is not EnemyEntity) return;

            EnemyData enemyData = entityTooltip.InspectedEntity.ComponentsContainer.Get<EnemyBootstrap>().EnemyData;
            
            if (_inspectedEnemyData.Contains(enemyData)) return;
            
            _inspectedEnemyData.Add(enemyData);
            
            SetProgress(_inspectedEnemyData.Count);

            DisableUI();
            
            await UniTask.WaitWhile(() => inspectionPanelBase);
            
            if (IsComplete) return;

            EnableUI();
        }
        
        public override async UniTask EndStep()
        {
            InspectionTooltipManager.Instance.OpenedTooltip -= TrySubscribeToEntityTooltipSync;
            
            await DisableUI();
        }
    }
}