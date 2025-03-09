using UnityEngine;
using Cashing;

public class AreaManager : MonoBehaviour
{
    [SerializeField] private AreaScanerController _mainScanerController;
    [Cached] protected ReachAreaScale _reachAreaScale;
    
    private void Start()
    {
        _reachAreaScale.ValueChanged += _ => UpdateScale();
        UpdateScale();
    }

    private void UpdateScale()
    {
        _mainScanerController.SetScale(_reachAreaScale.RoundedValue);
    }
}