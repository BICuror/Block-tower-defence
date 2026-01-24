using UnityEngine;
using Zenject;

namespace Combat
{
    public sealed class BuildingEntity : CombatEntity
    {
        [SerializeField] private bool _destroyOnDeath;
        
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

            if (!_destroyOnDeath) _waveStateMachine.StateStarted += RefillHealthOrRevive;
        }

        private void RefillHealthOrRevive(WaveState waveState)
        {
            if (waveState == WaveState.Idle)
            {
                if (_health.IsAlive()) _health.RefilHP();
            }
        }

        private void HandleDeathEvent()
        {
            if (_destroyOnDeath)
            {
                _health.Died -= HandleDeathEvent;
                Destroy(gameObject);
            }
            else gameObject.SetActive(false);
        }
    }
}