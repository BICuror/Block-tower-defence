using Ligofff.CustomSOIcons;
using CuroLocalization;
using UnityEngine;

public abstract class TooltipTagData : ScriptableObject
{
    [Tooltip("Tag without #")] [SerializeField] private string _tag;
    [SerializeField] private Color _textColor;
    [SerializeField] private Sprite _iconSprite;
    [SerializeField] private bool _onlyText;

    [Space] [Header("Localization")] 
    [SerializeField] private string _localizationKey;
    
    public string Tag => _tag;
    [CustomAssetIcon] public Sprite IconSprite => _iconSprite;
    public bool OnlyText => _onlyText;
    public Color TextColor => _textColor;
    public string TagText => (_localizationKey + "_header").Localize();
    public string Description => (_localizationKey + "_description").Localize();
}