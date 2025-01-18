using UnityEngine;

public abstract class SelectionOptionObject<T> : MonoBehaviour
{
    public abstract void SetTarget(T target);

    public abstract void ApplyDataToTarget();
}