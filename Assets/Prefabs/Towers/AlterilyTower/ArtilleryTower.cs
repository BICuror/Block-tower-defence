using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Cashing;
using Combat;

public sealed class ArtilleryTower : MonoBehaviour, ITaskConditionProvider
{
    [Inject] private DraggableCreator _draggableCreator;
    
    [Cached] private CombatEntity _ownerEntity;
    [Cached] private MaxEntities _maxEntities;
    [SerializeField] private ArtilleryTarget _artilleryTarget;
    private List<ArtilleryTarget> _createdTargets = new();

    private void Start()
    {
        _maxEntities.ValueChanged += _ => UpdateTargets();
        UpdateTargets();
    }
    
    ResolveTaskCondition ITaskConditionProvider.GetTaskCondition() => HasEnemiesInTargets;

    private bool HasEnemiesInTargets()
    {
        return _createdTargets.Exists(target => !target.AreaEntityDetector.IsEmpty);
    }
    
    private void UpdateTargets()
    {
        if (_createdTargets.Count < _maxEntities.RoundedValue)
        {
            int difference = _maxEntities.RoundedValue - _createdTargets.Count;

            for (int i = 0; i < difference; i++) AddTarget().Forget();
        }
        else if (_createdTargets.Count > _maxEntities.RoundedValue)
        {
            int difference = _createdTargets.Count - _maxEntities.RoundedValue;

            for (int i = 0; i < difference; i++) RemoveTarget();
        }
    }

    public async UniTask AddTarget()
    {
        ArtilleryTarget target = (await _draggableCreator.CreateDraggableOnRandomPosition(_artilleryTarget.GetComponent<DraggableObject>(), transform.position)).GetComponent<ArtilleryTarget>();
        target.AreaEntityAdded += _ownerEntity.ComponentsContainer.Get<TaskCycle>().TryCycle;
        
        target.Initialize(_ownerEntity);
        
        _createdTargets.Add(target);
    }

    public void RemoveTarget()
    {
        ArtilleryTarget target = _createdTargets[^1];
        target.AreaEntityAdded -= _ownerEntity.ComponentsContainer.Get<TaskCycle>().TryCycle;
        
        _createdTargets.Remove(target);
        
        Destroy(target.gameObject);
    }

    private void OnDestroy()
    {
        _createdTargets.ForEach(target => Destroy(target.gameObject));
    }
}