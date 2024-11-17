using System;
using Cashing;
using UnityEngine;

namespace Combat
{
    public class BuildingHealth : EntityHealth
    {
        //public Action BuildingDeathEvent;
    
        protected void Start()
        {
            Healed += CheckToHideHealthBar;
            Damaged += CheckToActivateHealthBar;
    
            //_healthBar.gameObject.SetActive(false);
    
            //Building building = GetComponent<Building>();
    
            //building.PickedUp.AddListener(DisableHealthBar);
            //building.Placed.AddListener(CheckToActivateHealthBar);
    
            //building.PickedUp.AddListener(SetInvincible);
            //building.Placed.AddListener(SetVulnerable);
        }
    
        //public void SetInvincible() => SetInvincibleState(true);
        //public void SetVulnerable() => SetInvincibleState(false);
    
        private void CheckToHideHealthBar()
        {
            //if (_healthBar.gameObject.activeSelf == true && GetHealthPrcentage() == 1f) DisableHealthBar();
        }
    
        private void CheckToActivateHealthBar()
        {
            //if (_healthBar.gameObject.activeSelf == false && GetHealthPrcentage() < 1f) EnableHealthBar();
        }
    
        public override void Die()
        {
            base.Die();
            //BuildingDeathEvent.Invoke(this);
            
            Destroy(gameObject);
        }
    }
}