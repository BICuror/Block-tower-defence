using Cysharp.Threading.Tasks;
using GameControls.Features;
using WorldGeneration;
using GameControls;
using Zenject;
using Combat;

namespace Tutorial.Custom
{
    public sealed class AttackStateTutorialStep : ProgressTutorialStep
    {
        [Inject] private GlobalEnemyContainer _globalEnemyContainer;
        [Inject] private EnemyBiomeContainer _enemyBiomeContainer;
        [Inject] private GameController _gameController;
        
        public override async UniTask StartStep()
        {
            int totalEnemiesToSpawn = 0;
            
            foreach (EnemyBiome enemyBiome in _enemyBiomeContainer.EnemyBiomeList)
            {
                totalEnemiesToSpawn += enemyBiome.EnemySpawner.EntitiesAmountToSpawn;
            }
            
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