using System.Collections.Generic;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class BombCreatorTower : MonoBehaviour, ITaskConditionProvider
{
    [Inject] private WaveStateMachine _waveStateMachine;
    [Inject] private DraggableCreator _draggableCreator;
    [SerializeField] private Bomb _bombPrefab;
    [Cached] private MaxEntities _maxEntities;
    [Cached] private BuildingEntity _ownerEntity;
    [Cached] private TaskCycle _taskCycle;
    private List<Bomb> _createdBombs = new();
    private WeaponPool<Bomb> _bombPool;

    public int ActiveBombs => _createdBombs.Count;
    
    private void Start()
    {
        _bombPool = new WeaponPool<Bomb>(_bombPrefab, 5, _ownerEntity, isFreeElement: IsFreeBomb);
        _taskCycle.TaskPerformed += CreateBomb;
        _waveStateMachine.StateStarted += HandleWaveStateChange;
    }

    private void HandleWaveStateChange(WaveState waveState)
    {
        if (waveState == WaveState.Attack)
        {
            _taskCycle.TryCycle();
        }
        else
        {
            for (int i = 0; i < _bombPool.Pool.Count; i++)
            {
                if (_bombPool.Pool[i].DraggableObject.IsPlaced)
                {
                    _bombPool.Pool[i].gameObject.SetActive(false);
                }
            }
        }
    }
    
    public ResolveTaskCondition GetTaskCondition() => LessThanMaxBombs;

    public async void CreateBomb()
    {
        Bomb bomb = _bombPool.GetPooledWeapon();
        bomb.Exploded += RemoveDisabledBomb;
        _createdBombs.Add(bomb);
        
        bomb.gameObject.SetActive(false);
        
        await _draggableCreator.ActivateDraggableOnRandomPosition(bomb.DraggableObject, transform.position, 2);
        
        bomb.EnableExplotion();
    }

    private void RemoveDisabledBomb(Bomb bomb)
    {
        bomb.Exploded -= RemoveDisabledBomb;
        _createdBombs.Remove(bomb);
        _taskCycle.TryCycle();
    }

    private bool IsFreeBomb(Bomb bomb) => bomb.IsFree;
    
    private bool LessThanMaxBombs() => _createdBombs.Count < _maxEntities.RoundedValue && _waveStateMachine.CurrentState == WaveState.Attack;

    private void OnDestroy()
    {
        _bombPool.DestroyPool();
        _waveStateMachine.StateStarted -= HandleWaveStateChange;
    } 
}