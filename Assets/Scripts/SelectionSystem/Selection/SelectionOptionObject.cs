using UnityEngine;
using System;

public sealed class SelectionOptionObject : MonoBehaviour, IActivatable
{
    private SelectionOptionObjectState _state;

    public Action Selected;

    void IActivatable.Activate()
    {
        if (_state == SelectionOptionObjectState.Idle)
        {
            Selected.Invoke();
            SetState(SelectionOptionObjectState.Activated);
        }
    }

    public void SetState(SelectionOptionObjectState state) => _state = state; 
}

public enum SelectionOptionObjectState
{
    Appearing,
    Idle,
    Activated,
    Disappearing
}
