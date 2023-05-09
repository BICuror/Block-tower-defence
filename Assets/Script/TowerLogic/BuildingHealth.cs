using UnityEngine;

public sealed class BuildingHealth : EntityHealth
{
    private void Start()
    {
        HealthChanged.AddListener(CheckToHideHealthBar);

        _healthBar.gameObject.SetActive(false);
    }

    private void CheckToHideHealthBar()
    {
        float value = GetHealthPrcentage();

        if (value == 1 && _healthBar.gameObject.activeSelf == true) _healthBar.gameObject.SetActive(false);
        else if (_healthBar.gameObject.activeSelf == false) _healthBar.gameObject.SetActive(true);
    }
}
