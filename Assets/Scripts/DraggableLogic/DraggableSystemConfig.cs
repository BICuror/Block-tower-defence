using UnityEngine;

[CreateAssetMenu(fileName = "DraggableSystemConfig", menuName = "DraggableSystem/DraggableSystemConfig")]

public sealed class DraggableSystemConfig : ScriptableObject
{
    [Header("VisualisationAppearAnimation")]
    [SerializeField] private float _areaVisualisationAppearDuration;
    [SerializeField] private AnimationCurve _areaVisualisationAppearCurve;      
    
    [Header("VisualisationDisappearAnimation")]
    [SerializeField] private float _areaVisualisationDisappearDuration;
    [SerializeField] private AnimationCurve _areaVisualisationDisappearCurve;

    [Header("AreaVisualisation")] [SerializeField] private float _additionalAreaVisualisationSize;
    
    public float AreaVisualisationAppearDuration => _areaVisualisationAppearDuration;
    public AnimationCurve AreaVisualisationAppearCurve => _areaVisualisationAppearCurve;
    
    public float AreaVisualisationDisappearDuration => _areaVisualisationDisappearDuration;
    public AnimationCurve AreaVisualisationDisappearCurve => _areaVisualisationDisappearCurve;
    
    public float AdditionalAreaVisualisationSize => _additionalAreaVisualisationSize;
}