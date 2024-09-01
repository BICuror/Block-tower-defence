public sealed class StaticUIElement : IngameUIElement
{
    private void Start()
    {
        IngameUIElementManager.Instance.AddStaticUIElement(this);
    }

    public void RotateTowardsCamera()
    {
        IngameUIElementManager.Instance.RotateElement(this);
    }

    public void OnEnable() => RotateTowardsCamera();

    private void OnDestroy() 
    {
        IngameUIElementManager.Instance.RemoveStaticUIElement(this);
    }
}
