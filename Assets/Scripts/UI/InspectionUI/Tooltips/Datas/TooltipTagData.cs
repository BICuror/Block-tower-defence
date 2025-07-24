using Ligofff.CustomSOIcons;
using UnityEngine;

public abstract class TooltipTagData : ScriptableObject
{
    [Tooltip("Tag without #")] [SerializeField] private string _tag;
    [Tooltip("Value which will be shown in final text")] [SerializeField] private string _tagText;
    [SerializeField] private Color _textColor;
    [SerializeField] private Sprite _iconSprite;
    [TextArea] [SerializeField] private string _description;
    
    public string Tag => _tag;
    public string TagText => _tagText;
    [CustomAssetIcon] public Sprite IconSprite => _iconSprite;
    public string Description => _description;
    public Color TextColor => _textColor;
}
