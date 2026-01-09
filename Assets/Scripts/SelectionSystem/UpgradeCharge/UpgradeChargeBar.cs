using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class UpgradeChargeBar : ProgressBarBase
{
    [SerializeField] private VisualEffectHandler _visualEffectHandler;
    [SerializeField] private int _chargesPerUpgrade;
    private float _previousValue;
    
    protected override string ProgressFieldName => "BuildProgress";
    protected override void OnFillComplete() {}

    private void Awake() => Initialize();
    
    public void ResetBar()
    {
        _previousValue = 0;
        gameObject.SetActive(true);
        StopBarFill();
        Shake();
        FillBar(0f, 0f, 0f).Forget();
        _visualEffectHandler.PlayBurstEffectAndForget();
    }

    public async UniTask SetCharges(int charges, float duration)
    {
        float barValue = charges * (1f / _chargesPerUpgrade);
        StopBarFill();
        
        await FillBar(_previousValue, barValue, duration);
        _previousValue = barValue;
    }
}