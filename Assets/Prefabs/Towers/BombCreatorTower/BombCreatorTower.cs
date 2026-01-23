using System.Collections.Generic;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class BombCreatorTower : MonoBehaviour, ITaskConditionProvider
{
    [SerializeField] private Bomb _bombPrefab;
    [Inject] private WaveStateMachine _waveStateMachine;
    [Inject] private DraggableCreator _draggableCreator;
    [Cached] private MaxEntities _maxEntities;
    [Cached] private BuildingEntity _ownerEntity;
    [Cached] private TaskCycle _taskCycle;
    private List<Bomb> _createdBombs = new();
    private WeaponPool<Bomb> _bombPool;
    
    public readonly OverridableBehaviour<Vector3> BombExploded = new();

    public int ActiveBombs => _createdBombs.Count;
    
    private void Start()
    {
        _bombPool = new WeaponPool<Bomb>(_bombPrefab, 5, _ownerEntity, isFreeElement: IsFreeBomb);
        _taskCycle.TaskPerformed += CreateBomb;
        _waveStateMachine.StateStarted += HandleWaveStateChange;
        
        BombExploded.Initialize(_ownerEntity);
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
                    _bombPool.Pool[i].StartExplosionAsync();
                }
            }
        }
    }
    
    public ResolveTaskCondition GetTaskCondition() => LessThanMaxBombs;

    public async void CreateBomb()
    {
        Bomb bomb = _bombPool.GetPooledWeapon();
        
        bomb.ExplosionStarted += InvokeBombExploded;
        bomb.ExplosionFinished += RemoveDisabledBomb;
        
        _createdBombs.Add(bomb);
        
        bomb.gameObject.SetActive(false);
        
        await _draggableCreator.ActivateDraggableOnRandomPosition(bomb.DraggableObject, transform.position, 2);
        
        bomb.EnableExplosion();
    }

    private void InvokeBombExploded(Bomb bomb) => BombExploded.Execute(bomb.transform.position);

    private void RemoveDisabledBomb(Bomb bomb)
    {
        bomb.ExplosionStarted -= InvokeBombExploded;
        bomb.ExplosionFinished -= RemoveDisabledBomb;
        
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