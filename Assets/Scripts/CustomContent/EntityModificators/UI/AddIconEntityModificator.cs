public sealed class AddIconEntityModificator : EntityModificator
{
    protected EntityCanvasIcon _icon;
    
    public override void Enable() => _icon = AddIcon(false);

    public override void Disable() => RemoveIcon(_icon);
}