public sealed class DynamicUIElement : IngameUIElement
{
    private void OnEnable()
    {
        IngameUIElementManager.Instance.AddDynamicUIElement(this);
    }

    private void OnDisable() 
    {
        IngameUIElementManager.Instance.RemoveDynamicUIElement(this);
    }
}
