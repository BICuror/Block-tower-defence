using UnityEngine;

public abstract class SelectionOptionData : ScriptableObject
{
    [Header("DefaultSettings")]
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private bool _shouldBeRemovedFromPoolAfterPick;

    public string Name => _name;
    public string Description => _description;
    public bool ShouldBeRemovedFromPool => _shouldBeRemovedFromPoolAfterPick;
}