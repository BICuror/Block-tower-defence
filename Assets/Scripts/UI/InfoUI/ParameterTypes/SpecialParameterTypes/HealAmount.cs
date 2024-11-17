using UnityEngine;

[CreateAssetMenu(fileName = "HealAmount", menuName = "Parameter/Special/HealAmount")]

public sealed class HealAmount : Parameter
{
    public override string GetValue(GameObject inspectable)
    {
        return "9";
    }
}
