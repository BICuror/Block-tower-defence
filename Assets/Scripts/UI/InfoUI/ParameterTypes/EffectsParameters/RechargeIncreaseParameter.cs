using UnityEngine;

[CreateAssetMenu(fileName = "RechargeIncreaseParameter", menuName = "Parameter/EffectParameters/RechargeIncreaseParameter")]

public sealed class RechargeIncreaseParameter : Parameter
{
    [SerializeField] private WorkFasterEffect _workFasterEffect;

    public override string GetValue(GameObject inspectable)
    {
        return _workFasterEffect.EffectStrength.ToString();
    }
}
