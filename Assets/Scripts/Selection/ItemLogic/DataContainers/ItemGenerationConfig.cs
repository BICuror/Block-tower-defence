using UnityEngine;

[CreateAssetMenu(fileName = "ItemGenerationConfig", menuName = "Item/ItemGenerationConfig")]

public sealed class ItemGenerationConfig : ScriptableObject
{
    [SerializeField] private int _maxWave;
    [SerializeField] private AnimationCurve _amountCurve;
    [SerializeField] private AnimationCurve _maxStrengthCurve;
    [SerializeField] private AnimationCurve _qualityCurve;

    public int MaxWave => _maxWave;
    public AnimationCurve AmountCurve => _amountCurve;
    public AnimationCurve MaxStrengthCurve => _maxStrengthCurve;
    public AnimationCurve QualityCurve => _qualityCurve;
}