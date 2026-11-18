using Combat.IngameUI.Inspection.InspectionCanvas;
using Combat.IngameUI.Inspection;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Combat.EnemySpawnSystem;
using Combat.Enemies;
using Combat;

namespace Tutorial.Custom
{
    public sealed class EnemyInspectionTutorialStep : ProgressTutorialStep
    {
        private List<EnemyConfig> _inspectedEnemyData = new();
        
        public override async UniTask StartStep()
        {
            InspectionCanvasManager.Instance.OpenedTooltip += TrySubscribeToEntityTooltipSync;
            
            await EnableUI();
        }

        private void TrySubscribeToEntityTooltipSync(InspectionCanvasBase inspectionCanvasBase) => TrySubscribeToEntityTooltip(inspectionCanvasBase).Forget();
        private async UniTask TrySubscribeToEntityTooltip(InspectionCanvasBase inspectionCanvasBase)
        {
            if (inspectionCanvasBase is not EntityInspectionCanvas) return;

            EntityInspectionCanvas entityInspectionCanvas = inspectionCanvasBase as EntityInspectionCanvas;

            if (entityInspectionCanvas.InspectedEntity is not EnemyEntity) return;

            EnemyConfig enemyConfig = entityInspectionCanvas.InspectedEntity.ComponentsContainer.Get<EnemyBootstrap>().EnemyConfig;
            
            if (_inspectedEnemyData.Contains(enemyConfig)) return;
            
            _inspectedEnemyData.Add(enemyConfig);
            
            SetProgress(_inspectedEnemyData.Count);

            DisableUI();
            
            await UniTask.WaitWhile(() => inspectionCanvasBase);
            
            if (IsComplete) return;

            EnableUI();
        }
        
        public override async UniTask EndStep()
        {
            InspectionCanvasManager.Instance.OpenedTooltip -= TrySubscribeToEntityTooltipSync;
            
            await DisableUI();
        }
    }
}