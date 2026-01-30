using UnityEngine;
using Zenject;

namespace Combat
{
    public sealed class BuildingEntity : CombatEntity
    {
        [SerializeField] private bool _destroyOnDeath;
        
        [Inject] private WaveStateMachine _waveStateMachine;
        private BuildingHealth _health;

        public bool IsDestroyedOnDeath => _destroyOnDeath;
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

            if (!_destroyOnDeath) _waveStateMachine.GetWaveStateController(WaveState.Idle).EnteredStateStarted += RefillHealthOrRevive;
        }

        private void RefillHealthOrRevive()
        {
            if (_health.IsAlive()) _health.RefilHP();
            else _health.ReviveBuilding();
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