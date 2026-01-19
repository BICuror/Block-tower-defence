using UnityEngine.Events;
using UnityEngine;

public sealed class ActivatableObject : MonoBehaviour, IActivatable
{
    public UnityEvent OnActivation;
    
    public void Activate()
    {
        OnActivation?.Invoke();
    }
}