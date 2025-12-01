public record SelectionSettings
{
    public SelectionSettings(SelectionType selectionType, object selectionArgument)
    {
        Type = selectionType;
        SelectionArgument = selectionArgument;
    }
    
    public SelectionSettings(SelectionType selectionType)
    {
        Type = selectionType;
    }

    public SelectionType Type;
    public object SelectionArgument;
    
    public T GetSelectionArgument<T>() => (T)SelectionArgument;
}
