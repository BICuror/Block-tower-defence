public class StaticUIElement : IngameUIElement
{
    protected void Start()
    {
        IngameUIElementManager.Instance.AddStaticUIElement(this);
    }
    
    private void OnDestroy() 
    {
        IngameUIElementManager.Instance.RemoveStaticUIElement(this);
    }
}