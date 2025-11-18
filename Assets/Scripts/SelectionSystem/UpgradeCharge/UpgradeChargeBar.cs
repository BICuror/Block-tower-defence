using TMPEffects.SerializedCollections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class UpgradeChargeBar : ProgressBarBase
{
    [SerializeField] private SerializedDictionary<int, float> _barFillValues;
    private float _previousValue;
    
    protected override string ProgressFieldName => "BuildProgress";
    protected override void OnFillComplete() {}

    public void ResetBar()
    {
        _previousValue = 0;
        gameObject.SetActive(true);
        StopBarFill();
        Shake();
    }

    public async UniTask SetCharges(int charges, float duration)
    {
        float barValue = _barFillValues[charges];
        StopBarFill();
        await FillBar(_previousValue, barValue, duration);
        _previousValue = barValue;
    }
}