using UnityEngine;
using System;

public sealed class EntityHealthBar : HealthBar
{
    [SerializeField] private bool _alwaysShow = false;
    private bool _isInitialized;

    public bool IsActive => !OwnerHealth.IsFullHp() || _alwaysShow;
    
    public event Action HealthBarStateUpdated;
    
    private void Start()
    {
        Initialize();
        
        OwnerHealth.Healed += UpdateHealthBarState;
        OwnerHealth.Damaged += UpdateHealthBarState;
        
        OwnerHealth.Damaged += UpdateBar;
        OwnerHealth.Healed += UpdateBar;
        
        
        if (!_alwaysShow) gameObject.SetActive(false);

        _isInitialized = true;
    }

    private void UpdateHealthBarState()
    {
        bool healthBarState = IsActive;
        
        gameObject.SetActive(healthBarState);
        
        if (gameObject.activeSelf != healthBarState) HealthBarStateUpdated?.Invoke();
    }

    private void OnDestroy()
    {
        base.OnDestroy();
        
        OwnerHealth.Healed -= UpdateHealthBarState;
        OwnerHealth.Damaged -= UpdateHealthBarState;
        
        OwnerHealth.Damaged -= UpdateBar;
        OwnerHealth.Healed -= UpdateBar;
        
    }
}