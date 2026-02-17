using UnityEngine.Events;
using UnityEngine;

public sealed class UIElementKeyboardController : MonoBehaviour
{
    private UIElementControls _uiElementControls;

    public UnityEvent Quit;

    private void Awake()
    {
        _uiElementControls = new UIElementControls();
        _uiElementControls.Enable();

        _uiElementControls.Main.Quit.started += _ => QuitUI();
    }
    
    private void QuitUI()
    {
        Quit.Invoke();
    }

    private void OnDestroy()
    {
        _uiElementControls.Disable();
        _uiElementControls.Dispose();
    }
}