using Cysharp.Threading.Tasks;
using Zenject;

namespace Combat
{
    public sealed class BuildingEntity : CombatEntity
    {
        [Inject] private WaveStateMachine _waveStateMachine;
        private BuildingHealth _health;

        public BuildingHealth BuildingHealth => _health;

        private void Awake()
        {
            _health = new();
            ComponentsContainer.Add<BuildingHealth>(_health);
            ComponentsContainer.Add<EntityHealth>(_health);
            
            base.Awake();
            
            InjectCached(_health);
            _health.Initialize();
            _health.RefilHP();
            _health.Died += HandleDeathEvent;
        }

        private void HandleDeathEvent()
        {
            gameObject.SetActive(false);
            
            DestroyOnWaveEnd().Forget();
        }

        private async UniTask DestroyOnWaveEnd()
        {
            await UniTask.WaitUntil(() => _waveStateMachine.CurrentState == WaveState.Idle);
            
            Destroy(gameObject);
        }
    }
}