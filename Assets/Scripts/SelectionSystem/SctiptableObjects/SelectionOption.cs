using UnityEngine;

public abstract class SelectionOption : ScriptableObject 
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private string _description;
    public string Description => _description;

    [SerializeField] private bool _shouldBeRemovedAfterTaking;
    public bool ShouldBeRemovedAfterTaking => _shouldBeRemovedAfterTaking;
}
