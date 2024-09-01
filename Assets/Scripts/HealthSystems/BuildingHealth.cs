using UnityEngine;
using UnityEngine.Events;

public class BuildingHealth : EntityHealth
{
    public UnityEvent<BuildingHealth> BuildingDeathEvent;

    protected void Start()
    {
        Healed.AddListener(CheckToHideHealthBar);
        Damaged.AddListener(CheckToActivateHealthBar);

        _healthBar.gameObject.SetActive(false);

        Building building = GetComponent<Building>();

        building.PickedUp.AddListener(DisableHealthBar);
        building.Placed.AddListener(CheckToActivateHealthBar);

        building.PickedUp.AddListener(SetInvincible);
        building.Placed.AddListener(SetVulnerable);
    }

    public void SetInvincible() => SetInvincibleState(true);
    public void SetVulnerable() => SetInvincibleState(false);

    private void CheckToHideHealthBar()
    {
        if (_healthBar.gameObject.activeSelf == true && GetHealthPrcentage() == 1f) DisableHealthBar();
    }

    private void CheckToActivateHealthBar()
    {
        if (_healthBar.gameObject.activeSelf == false && GetHealthPrcentage() < 1f) EnableHealthBar();
    }

    public override void Die()
    {
        DeathEvent.Invoke(gameObject);

        BuildingDeathEvent.Invoke(this);
        
        Destroy(gameObject);
    }
}
