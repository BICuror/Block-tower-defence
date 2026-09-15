using UnityEngine;

public abstract class SelectionOptionObject : MonoBehaviour
{
    public abstract void ApplySelectedEffect();

    public virtual void OnObjectCreationCompleted() {}
}