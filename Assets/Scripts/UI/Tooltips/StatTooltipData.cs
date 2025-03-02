using UnityEngine;

[CreateAssetMenu(menuName = "UI/Tooltips/StatTooltipData")]

public sealed class StatTooltipData : ScriptableObject
{
    [SerializeField] private string _associatedStatTypeName;
    [SerializeField] private string _statName;
    [TextArea] [SerializeField] private string _description;
    [SerializeField] private Sprite _iconSprite;
    [SerializeField] private string _tmpSpriteName;
    
    public string AssociatedStatTypeName => _associatedStatTypeName;
    public string StatName => _statName;
    public string Description => _description;
    public Sprite IconSprite => _iconSprite;
    public string TMPSpriteName => _tmpSpriteName;
}