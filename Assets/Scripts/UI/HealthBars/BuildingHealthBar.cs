public sealed class BuildingHealthBar : HealthBar
{
    private void Start()
    {
        base.Start();

        EntityHealth.Healed += TryHideBar;
        EntityHealth.Damaged += ShowBar;
        
        gameObject.SetActive(false);
    }

    private void TryHideBar()
    {
        if (EntityHealth.IsFullHp())
        {
            gameObject.SetActive(false);
        }
    }

    private void ShowBar()
    {
        gameObject.SetActive(true);
    }
}