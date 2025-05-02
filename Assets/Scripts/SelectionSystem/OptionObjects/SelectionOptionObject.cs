using UnityEngine;

public abstract class SelectionOptionObject : MonoBehaviour
{
    public abstract string OptionDescription { get; }
    
    public abstract void ApplyEffect();
}