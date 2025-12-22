using UnityEngine;
using Zenject;

namespace Combat
{
    public sealed class Townhall : MonoBehaviour
    {
        [Inject] private WaveStateMachine _waveStateMachine;
        [Inject] private ItemFactory _itemFactory;
        [Inject] private WaveManager _waveManager;
        
        private void Awake()
        {
            _waveStateMachine.StateStarted += TryToSpawnCrystals;
        }
        
        public void SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition + Vector3.up;
        }

        private void TryToSpawnCrystals(WaveState currentWaveState)
        {
            if (currentWaveState == WaveState.Idle) CreateCrystals();
        }
        
        private void CreateCrystals()
        {
            _itemFactory.CreateStartWaveItem(transform.position);
            
            _itemFactory.CreateItem(4, transform.position);
            if (_waveManager.GetCurrentWave() > 1) _itemFactory.CreateItem(7, transform.position);
        }
    }
}