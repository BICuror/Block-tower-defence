using UnityEngine;
using UnityEngine.Events;
using Zenject;

public sealed class CrystalManager : MonoBehaviour
{
    public UnityEvent CrystalUsed;
    public UnityEvent CrystalSet;

    [SerializeField] private CrystalDetector _townhallCrystalDetector;

    private Crystal _currentCrystal;

    private void Awake() => _townhallCrystalDetector.PlacedComponentAdded.AddListener(SetCrystal);

    public void SetCrystal(Crystal crystal)
    {
        _currentCrystal = crystal;
        
        _currentCrystal.DisableDraggableComponent();

        _currentCrystal.CrystalUsed.AddListener(OnCrystalUsed);

        _currentCrystal.Activate();

        CrystalSet.Invoke();
    }

    private void OnCrystalUsed()
    {
        _currentCrystal.CrystalUsed.RemoveListener(OnCrystalUsed);

        CrystalUsed.Invoke();
    }

    public void DestroyCrystal()
    {
        _currentCrystal.Destroy();
    }
}
