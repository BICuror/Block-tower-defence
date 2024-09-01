using UnityEngine;

[CreateAssetMenu(fileName = "Unlock", menuName = "Unlocks/UnlockedByDefault")]

public class Unlock : ScriptableObject 
{
    public virtual string GetRequirement() => null;
    public virtual bool IsUnlocked() => true;
}
