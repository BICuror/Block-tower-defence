using UnityEngine;

public abstract class Parameter : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private Sprite _icon;
    public Sprite Icon => _icon;

    [SerializeField] private Color _valueColor;
    public Color ValueColor => _valueColor;

    public abstract string GetValue(GameObject inspectable);
}
