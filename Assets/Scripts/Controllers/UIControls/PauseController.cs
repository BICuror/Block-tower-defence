using UnityEngine.Events;
using UnityEngine;

public sealed class PauseController : MonoBehaviour
{
    public UnityEvent PauseEnabled;

    public UnityEvent PauseDisabled;

    private UIControls _controls;

    private float _recordedTimScale;
    
    public void TogglePause()
    {
        if (Time.timeScale == 0f) DisablePause();
        else EnablePause();
    }

    public void EnablePause()
    {   
        _recordedTimScale = Time.timeScale;

        Time.timeScale = 0f;

        PauseEnabled.Invoke();
    }

    public void DisablePause()
    {
        Time.timeScale = _recordedTimScale;

        PauseDisabled.Invoke();
    }

    #region Enable/Disable
    private void Awake() 
    {
        Time.timeScale = 1f;

        CreateControls();

        Enable();
    }

    private void OnDestroy() 
    {
        Disable();

        _controls = null;

        Time.timeScale = 1f;
    }

    private void CreateControls()
    {
        _controls = new UIControls();

        _controls.TouchInput.EscapeToggle.performed += _ => TogglePause();
    }

    public void Enable() => _controls.Enable();
    public void Disable() => _controls.Disable();
    #endregion
}
