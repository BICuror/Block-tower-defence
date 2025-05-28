using System.Collections.Generic;
using UnityEngine;
using Cashing;

public class AreaManager : MonoBehaviour
{
    [SerializeField] private List<AreaScanerController> _mainScanerControllers;
    [Cached] protected ReachAreaScale _reachAreaScale;
    
    private void Start()
    {
        _reachAreaScale.ValueChanged += _ => UpdateScale();
        UpdateScale();
    }

    private void UpdateScale()
    {
        _mainScanerControllers.ForEach(scanerContoller => scanerContoller.SetScale(_reachAreaScale.RoundedValue));
    }

    public void AddAreaScanerController(AreaScanerController areaScanerController)
    {
        _mainScanerControllers.Add(areaScanerController);
        UpdateScale();
    }

    public void RemoveAreaScanerController(AreaScanerController areaScanerController)
    {
        _mainScanerControllers.Remove(areaScanerController);
    }
}