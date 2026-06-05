using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public sealed class EntityCanvas : IngameUIElement
{
    [SerializeField] private Transform _customContentParent;
    [SerializeField] private float _itemsSpacing;

    [Header("HealthBar")]
    [SerializeField] private float _healthBarHeight = 1.5f;
    [SerializeField] private bool _includeHealthBarToCustomContent = true;
    [ShowIf("_includeHealthBarToCustomContent")] [SerializeField] private EntityHealthBar _healthBar;
    
    [Header("Bars")]
    [SerializeField] private float _barHeight = 1.5f;
    [SerializeField] private EntityCanvasBar _entityCanvasBarPrefab;
    private List<EntityCanvasBar> _bars = new();
    
    [Header("Icons")]
    [SerializeField] private float _iconsWidth = 1f;
    [SerializeField] private float _iconsBarHeight = 1f;
    [SerializeField] private float _iconsSpacing = 1f;
    [SerializeField] private EntityCanvasIcon _entityCanvasIconPrefab;
    [SerializeField] private Transform _canvasIconParent;
    private List<EntityCanvasIcon> _icons = new();

    [Header("AbilityIcons")]
    [SerializeField] private float _abilityIconsHeight = 1.5f;
    [SerializeField] private float _abilityIconsSpacing = 1.5f;
    [SerializeField] private EntityCanvasAbilityIcon _entityCanvasAbilityIconPrefab;
    private List<EntityCanvasAbilityIcon> _abilityIcons = new();
    
    private void Start()
    {
        _healthBar.HealthBarStateUpdated += UpdateCanvasLayout;
        UpdateCanvasLayout();
    }

    public EntityCanvasBar AddBar(Sprite barIconSprite, float value, EntityCanvasBar customBarPrefab = null)
    {
        EntityCanvasBar barPrefab = customBarPrefab ?? _entityCanvasBarPrefab; 
        
        EntityCanvasBar bar = Instantiate(barPrefab, _customContentParent);
        bar.Initialize(barIconSprite, value);
        
        _bars.Add(bar);

        UpdateCanvasLayout();
        
        return bar;
    }

    public void RemoveBar(EntityCanvasBar bar)
    {
        _bars.Remove(bar);
        Destroy(bar.gameObject);
        UpdateCanvasLayout();
    }

    public EntityCanvasIcon AddIcon(Sprite iconSprite, bool hasValue = false, int value = 0, EntityCanvasIcon customIconPrefab = null)
    {
        EntityCanvasIcon iconPrefab = customIconPrefab ?? _entityCanvasIconPrefab; 

        EntityCanvasIcon icon = Instantiate(iconPrefab, _canvasIconParent);
        icon.Initialize(iconSprite, hasValue, value);
        
        _icons.Add(icon);
        
        UpdateIconsLayout();         
        
        return icon;
    }

    public void RemoveIcon(EntityCanvasIcon icon)
    {
        _icons.Remove(icon);
        Destroy(icon.gameObject);
        
        UpdateIconsLayout();    
    }

    public EntityCanvasAbilityIcon AddAbilityIcon(Sprite iconSprite, float value, EntityCanvasAbilityIcon customAbilityIconPrefab = null)
    {
        EntityCanvasAbilityIcon abilityIconPrefab = customAbilityIconPrefab ?? _entityCanvasAbilityIconPrefab; 

        EntityCanvasAbilityIcon icon = Instantiate(abilityIconPrefab, _customContentParent);
        icon.StateUpdated += UpdateCanvasLayout;
        icon.Initialize(iconSprite, value);
        
        _abilityIcons.Add(icon);
        
        UpdateCanvasLayout();         
        
        return icon;
    }
    
    public void RemoveAbilityIcon(EntityCanvasAbilityIcon icon)
    {
        icon.StateUpdated -= UpdateCanvasLayout;
        _abilityIcons.Remove(icon);
        Destroy(icon.gameObject);
        
        UpdateCanvasLayout();    
    }
    
    [Button] public void UpdateCanvasLayout()
    {
        float currentHeight = 0f;
        
        if (_healthBar.IsActive) currentHeight += _healthBarHeight + _itemsSpacing;
        
        for (int i = 0; i < _bars.Count; i++)
        {
            _bars[i].transform.localPosition = new Vector3(0f, 0f, -currentHeight);
            
            currentHeight += _barHeight + _itemsSpacing;
        }

        for (int i = 0; i < _abilityIcons.Count; i++)
        {
            if (!_abilityIcons[i].IsActive) continue;
            
            currentHeight += _abilityIconsHeight;
            
            _abilityIcons[i].transform.localPosition = new Vector3(0f, 0f, -currentHeight);
            
            currentHeight += _abilityIconsHeight + _abilityIconsSpacing;
        }
        
        _canvasIconParent.localPosition = new Vector3(0f, 0f, -currentHeight);
    }

    private void UpdateIconsLayout()
    {
        int iconsCount = _icons.Count;

        if (iconsCount > 1)
        {
            float width = iconsCount * _iconsWidth + (iconsCount - 1) * _iconsSpacing;
            float widthStep = width / (iconsCount - 1);
            float halfWidth = width / 2;
    
            for (int i = 0; i < iconsCount; i++)
            {
                _icons[i].transform.localPosition = new Vector3(widthStep * i - halfWidth, 0f, 0f);
            }
        }
        else if (iconsCount == 1)
        {
            _icons[0].transform.localPosition = Vector3.zero;
        }
    }
}