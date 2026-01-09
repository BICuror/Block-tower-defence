using UnityEngine.Events;
using UnityEngine;

public class HoverableObject : MonoBehaviour
{
    private bool _isHoveredOver;

    public UnityEvent HoverEntered;
    public UnityEvent HoverExited;
    
    protected virtual void OnHoverEnter() {}
    protected virtual void OnHoverExit() {}
    
    public void EnterHover()
    {
        _isHoveredOver = true;
        OnHoverEnter();
        HoverEntered?.Invoke();
        
        Debug.Log("Hover Entered");
    }

    public void ExitHover()
    {
        _isHoveredOver = false;
        OnHoverExit();
        HoverExited?.Invoke();
    }
    
    private void OnDisable() => _isHoveredOver = false;
}