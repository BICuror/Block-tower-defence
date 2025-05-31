using System.Collections.Generic;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class BombCreatorTower : MonoBehaviour, ITaskConditionProvider
{
    [Inject] private DraggableCreator _draggableCreator;
    [SerializeField] private Explotion _bombPrefab;
    [Cached] private MaxEntities _maxEntities;
    [Cached] private BuildingEntity _ownerEntity;
    [Cached] private TaskCycle _taskCycle;
    private List<Bomb> _createdBombs = new();
    private WeaponBasePool<Explotion> _bombPool;

    public int ActiveBombs => _createdBombs.Count;
    
    private void Start()
    {
        _bombPool = new WeaponBasePool<Explotion>(_bombPrefab, 5, _ownerEntity);
        _taskCycle.TaskPerformed += CreateBomb;
    }
    
    public ResolveTaskCondition GetTaskCondition() => LessThanMaxBombs;

    public async void CreateBomb()
    {
        Explotion explotion = _bombPool.GetPooledWeapon();
        Bomb bomb = explotion.GetComponent<Bomb>();
        bomb.Exploded += RemoveDisabledBomb;
        _createdBombs.Add(bomb);
        
        bomb.gameObject.SetActive(false);
        
        await _draggableCreator.ActivateDraggableOnRandomPosition(bomb, transform.position, 2);
        
        bomb.EnableExplotion();
    }

    private void RemoveDisabledBomb(Bomb bomb)
    {
        bomb.Exploded -= RemoveDisabledBomb;
        _createdBombs.Remove(bomb);
        _taskCycle.TryCycle();
    }
    
    private bool LessThanMaxBombs() => _createdBombs.Count <= _maxEntities.RoundedValue;

    private void OnDestroy() => _bombPool.DestroyPool();
}