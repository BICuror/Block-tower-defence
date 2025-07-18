using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class SelectionOptionObject : MonoBehaviour
{
    public abstract string OptionName { get; }
    public abstract string OptionDescription { get; }
    public abstract Sprite Icon { get; }
    
    public abstract void ApplyEffect();
}