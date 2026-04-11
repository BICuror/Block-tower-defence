using UnityEngine;

public abstract class FocusElement : MonoBehaviour
{
    private bool _focusValueUpdating;
    private float _focus;
    
    protected float Focus => _focus;
    
    public void SetFocusValue(float value)
    {
        _focus = value;
        
        OnFocusChanged();
    }
    
    public void UpdateViewportPosition(Vector2 position) => OnViewportPositionChanged(position);
    
    protected abstract void OnFocusChanged();
    protected virtual void OnViewportPositionChanged(Vector2 position) {}

    protected void StartUpdatingFocusValue()
    {
        if (_focusValueUpdating) return;
        
        _focusValueUpdating = true;
        
        FocusMannager.Instance.AddFocusElement(this);
    }
    
    public void StopUpdatingFocusValue()
    {
        if (!_focusValueUpdating) return;
        
        _focusValueUpdating = false;
        
        FocusMannager.Instance.RemoveFocusElement(this);
    }
}