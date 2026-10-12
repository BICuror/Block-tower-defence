using Combat.EnemySpawnSystem;
using Cysharp.Threading.Tasks;
using GameControls.Features;
using GameControls;
using Zenject;
using Combat;

namespace Tutorial.Custom
{
    public sealed class AttackStateTutorialStep : ProgressTutorialStep
    {
        [Inject] private EnemySpawnGroupContainer _enemySpawnGroupContainer;
        [Inject] private GlobalEnemyContainer _globalEnemyContainer;
        [Inject] private GameController _gameController;
        
        public override async UniTask StartStep()
        {
            int totalEnemiesToSpawn = _enemySpawnGroupContainer.GetTotalSpawnCount();
            
            SetRequiredProgress(totalEnemiesToSpawn);

            _globalEnemyContainer.EnemyRemoved += IncreaseProgress;

            _gameController.EnableFeature(ControllerFeature.TimeToggle);
            
            await EnableUI();
        }

        private void IncreaseProgress(EnemyEntity _) => IncreaseProgress();

        public override async UniTask EndStep()
        {
            _globalEnemyContainer.EnemyRemoved -= IncreaseProgress;
            
            await DisableUI();
        }
    }
}