using UnityEngine;
using Zenject;

public sealed class Chest : MonoBehaviour
{
    [Inject] private WaveStateMachine _waveStateController;
    [Inject] private ItemFactory _itemFactory;

    private void Awake()
    {
        _waveStateController.StateEnded += TryToCreateItem;
    }

    private void TryToCreateItem(WaveState waveState)
    {
        if (waveState == WaveState.Attack)
        {
            _itemFactory.CreateItem(1, 2, transform.position);
            
            _waveStateController.StateEnded -= TryToCreateItem;
            Destroy(gameObject);
        }
    }
}