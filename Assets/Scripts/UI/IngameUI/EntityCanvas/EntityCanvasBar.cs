using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class EntityCanvasBar : ProgressBarBase
{
    [SerializeField] private bool _shakeOnValueChange;
    [SerializeField] private SpriteRenderer _iconRenderer;
    [SerializeField] private float _tweenDuration = 0.2f;
    private float _lastAssignedValue;
    
    public void Initialize(Sprite iconSprite, float value)
    {
        base.Initialize();
        
        _iconRenderer.sprite = iconSprite;
        _lastAssignedValue = value;

        SetValue(value).Forget();
    }

    public async UniTask SetValue(float value)
    {
        if (_shakeOnValueChange) Shake();
        
        await FillBar(_lastAssignedValue, value, _tweenDuration);
        
        _lastAssignedValue = value;
    }
    
    protected override string ProgressFieldName => "Progress";

    protected override void OnFillComplete() {}
}
