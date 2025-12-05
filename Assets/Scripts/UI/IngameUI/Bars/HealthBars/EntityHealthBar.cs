using UnityEngine;

public sealed class EntityHealthBar : HealthBar
{
    [SerializeField] private bool _alwaysShow = false;
    private bool _isInitialized;

    public bool IsActive => !OwnerHealth.IsFullHp() || _alwaysShow;
    
    private void Start()
    {
        Initialize();
        
        OwnerHealth.Healed += TryHideBar;
        
        OwnerHealth.Damaged += UpdateBar;
        OwnerHealth.Healed += UpdateBar;
        
        OwnerHealth.Damaged += ShowBar;
        
        if (!_alwaysShow) gameObject.SetActive(false);

        _isInitialized = true;
    }

    private void TryHideBar()
    {
        if (IsActive) return;
        
        gameObject.SetActive(false);
    }

    private void ShowBar()
    {
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        base.OnDestroy();
        
        OwnerHealth.Healed -= TryHideBar;
        
        OwnerHealth.Damaged -= UpdateBar;
        OwnerHealth.Healed -= UpdateBar;
        
        OwnerHealth.Damaged -= ShowBar;
    }
}