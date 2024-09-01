using UnityEngine;

[CreateAssetMenu(fileName = "MaxHealth", menuName = "Parameter/MaxHealth")]

public sealed class MaxHealthParameter : Parameter
{
    public override string GetValue(GameObject inspectable)
    {
        return inspectable.GetComponent<EntityHealth>().GetMaxHealth().ToString();
    }
}
