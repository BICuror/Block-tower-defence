using UnityEngine;

public abstract class TooltipTagData : ScriptableObject
{
    [Tooltip("Tag without #")] [SerializeField] private string _tag;
    [Tooltip("Value which will be shown in final text")] [SerializeField] private string _finalText;
    [SerializeField] private Sprite _iconSprite;
    [TextArea] [SerializeField] private string _description;
    
    public string Tag => _tag;
    public string FinalText => _finalText;
    public Sprite IconSprite => _iconSprite;
    public string Description => _description;
}
