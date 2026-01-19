using System.Collections.Generic;
using UnityEngine;
using Cashing;
using Combat;

public class AreaManager : MonoBehaviour
{
    [SerializeField] private AreaEntityDetectorPriorityType _priorityType;
    [SerializeField] private List<AreaScanerController> _mainScanerControllers;
    [Cached] protected ReachAreaScale _reachAreaScale;
    
    public AreaEntityDetectorPriorityType CurrentPriorityType => _priorityType;
    public List<AreaScanerController> ControlledScanerControllers => _mainScanerControllers;
    
    private void Start()
    {
        _reachAreaScale.ValueChanged += _ => UpdateScale();
        UpdateScale();

        UpdateAllScanersPriorityAlgorithm();
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
        areaScanerController.GetComponent<AreaEntityDetector>().SetPriorityAlgorithm(priorityAlgorithm);
    }
    
    private EntityDetectorPriorityAlgorithm GetActivePriorityAlgorithm()
    {
        EntityDetectorPriorityAlgorithm priorityAlgorithm = null;        
        
        switch (_priorityType)
        {
            case AreaEntityDetectorPriorityType.None: priorityAlgorithm = null; break;
            case AreaEntityDetectorPriorityType.HighestHealth: priorityAlgorithm = new HighestHealthPriorityAlgorithm(); break;
            case AreaEntityDetectorPriorityType.LowestHealth: priorityAlgorithm = new LowestHealthPriorityAlgorithm(); break;
            case AreaEntityDetectorPriorityType.HighestMaxHealth: priorityAlgorithm = new HighestMaxHealthPriorityAlgorithm(); break;
            case AreaEntityDetectorPriorityType.LowestMaxHealth: priorityAlgorithm = new LowestMaxHealthPriorityAlgorithm(); break;
        }

        return priorityAlgorithm;
    }
}

public enum AreaEntityDetectorPriorityType
{
    None,
    HighestHealth,
    LowestHealth,
    HighestMaxHealth,
    LowestMaxHealth,
}

public abstract class EntityDetectorPriorityAlgorithm
{
    public abstract CombatEntity GetPrioritizedEntity(List<CombatEntity> initialList);
}