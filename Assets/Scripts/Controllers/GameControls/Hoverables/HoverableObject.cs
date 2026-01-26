using UnityEngine.Events;
using UnityEngine;

public class HoverableObject : MonoBehaviour
{
    private bool _isHoveredOver;

    public bool IsHoveredOver => _isHoveredOver;
    
    public UnityEvent HoverEntered;
    public UnityEvent HoverExited;
    
    public void EnterHover()
    {
        _isHoveredOver = true;
        HoverEntered?.Invoke();
        
        Debug.Log("Hover Entered");
    }

    public void ExitHover()
    {
        _isHoveredOver = false;
        HoverExited?.Invoke();
    }
}