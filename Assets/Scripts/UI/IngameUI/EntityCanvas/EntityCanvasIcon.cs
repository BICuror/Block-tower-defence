using UnityEngine;
using TMPro;

public sealed class EntityCanvasIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private TextMeshPro _iconValue;
    
    public void Initialize(Sprite sprite, bool hasValue, int value = 0)
    {
        _spriteRenderer.sprite = sprite;
        _iconValue.gameObject.SetActive(hasValue);
        if (hasValue) SetValue(value);
    }

    public void SetValue(int value)
    {
        _iconValue.text = value.ToString();
    }
}