using UnityEngine;

[CreateAssetMenu(fileName = "MortarExplotionDamage", menuName = "Parameter/Special/MortarExplotionDamage")]

public sealed class MortarExplotionDamageParameter : Parameter
{
    public override string GetValue(GameObject inspectable)
    {
        return inspectable.GetComponent<MortarTower>().ExplotionDamage.ToString();
    }
}