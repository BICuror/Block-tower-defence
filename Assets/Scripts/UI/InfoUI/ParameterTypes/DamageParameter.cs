using UnityEngine;

[CreateAssetMenu(fileName = "Damage", menuName = "Parameter/Damage")]

public sealed class DamageParameter : Parameter
{
    public override string GetValue(GameObject inspectable)
    {
        return inspectable.GetComponent<CombatBuilding>().GetDamageValue();
    }
}
