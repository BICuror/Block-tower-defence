using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Cashing;
using Combat;

public class AreaManager : MonoBehaviour
{
    
    [SerializeField] private List<AreaScanerController> _mainScanerControllers;
    
    [Header("Priority")] 
    [SerializeField] private bool _hasPriority;
    [ShowIf("_hasPriority")] [SerializeField] private AreaEntityDetectorPriorityType _priorityType;
    
    [Cached] protected ReachAreaScale _reachAreaScale;
    [Cached] protected DraggableObject _draggableObject;
    
    public bool HasPriority => _hasPriority;
    public AreaEntityDetectorPriorityType CurrentPriorityType => _priorityType;
    public List<AreaScanerController> ControlledScanerControllers => _mainScanerControllers;
    
    private void Start()
    {
        _reachAreaScale.ValueChanged += _ => UpdateScale();
        
        UpdateAllScanersPriorityAlgorithm();
        UpdateScale();
    }

    public void SetPriorityType(AreaEntityDetectorPriorityType priorityType)
    {
        _priorityType = priorityType;
        UpdateAllScanersPriorityAlgorithm();
    } 
    
    public void AddAreaScanerController(AreaScanerController areaScanerController)
    {
        _mainScanerControllers.Add(areaScanerController);
        TrySetEntityDetectorPriority(areaScanerController, GetActivePriorityAlgorithm());
        UpdateScale();
    }

    public void RemoveAreaScanerController(AreaScanerController areaScanerController)
    {
        _mainScanerControllers.Remove(areaScanerController);
    }

    private void UpdateScale()
    {
        _mainScanerControllers.ForEach(scanerContoller => scanerContoller.SetScale(_reachAreaScale.RoundedValue));
    }

    private void UpdateAllScanersPriorityAlgorithm()
    {
        EntityDetectorPriorityAlgorithm algorithm = GetActivePriorityAlgorithm();
        
        _mainScanerControllers.ForEach(scanerController => TrySetEntityDetectorPriority(scanerController, algorithm));
    }
    
    private void TrySetEntityDetectorPriority(AreaScanerController areaScanerController, EntityDetectorPriorityAlgorithm priorityAlgorithm)
    {
        if (areaScanerController.gameObject.TryGetComponent(out AreaEntityDetector detector))
        {
            detector.SetPriorityAlgorithm(priorityAlgorithm);   
        }
    }
    
    private EntityDetectorPriorityAlgorithm GetActivePriorityAlgorithm()
    {
        EntityDetectorPriorityAlgorithm priorityAlgorithm = null;        
        
        switch (_priorityType)
        {
            case AreaEntityDetectorPriorityType.First: priorityAlgorithm = null; break;
            case AreaEntityDetectorPriorityType.Last: priorityAlgorithm = new LastPriorityAlgorithm(); break;
            case AreaEntityDetectorPriorityType.HighestHealth: priorityAlgorithm = new HighestHealthPriorityAlgorithm(); break;
            case AreaEntityDetectorPriorityType.LowestHealth: priorityAlgorithm = new LowestHealthPriorityAlgorithm(); break;
            case AreaEntityDetectorPriorityType.HighestMaxHealth: priorityAlgorithm = new HighestMaxHealthPriorityAlgorithm(); break;
            case AreaEntityDetectorPriorityType.LowestMaxHealth: priorityAlgorithm = new LowestMaxHealthPriorityAlgorithm(); break;
        }

        return priorityAlgorithm;
    }

    private float GetOwnerScaleModifier()
    {
        if (_draggableObject) return (_draggableObject.TileScale - 1) * 0.5f;
        
        return 0;
    }
}

public enum AreaEntityDetectorPriorityType
{
    First = 0,
    Last = 1,
    HighestHealth = 2,
    LowestHealth = 3,
    HighestMaxHealth = 4,
    LowestMaxHealth = 5,
}

public abstract class EntityDetectorPriorityAlgorithm
{
    public abstract CombatEntity GetPrioritizedEntity(List<CombatEntity> initialList);
}