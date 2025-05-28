public sealed class BuildingHealthBar : HealthBar
{
    private void Start()
    {
        base.Start();
        
        OwnerHealth.Healed += TryHideBar;
        OwnerHealth.Damaged += ShowBar;
        
        gameObject.SetActive(false);
    }

    private void TryHideBar()
    {
        if (OwnerHealth.IsFullHp())
        {
            gameObject.SetActive(false);
        }
    }

    private void ShowBar()
    {
        gameObject.SetActive(true);
    }
}