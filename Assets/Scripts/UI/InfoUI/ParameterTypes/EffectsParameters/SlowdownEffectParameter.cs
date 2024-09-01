using UnityEngine;

[CreateAssetMenu(fileName = "SlowdownEffectParameter", menuName = "Parameter/EffectParameters/SlowdownEffectParameter")]

public sealed class SlowdownEffectParameter : Parameter
{
    [SerializeField] private SlowdownEffect _slowdownEffect;

    public override string GetValue(GameObject inspectable)
    {
        return _slowdownEffect.SlowdownStrength.ToString();
    }
}
