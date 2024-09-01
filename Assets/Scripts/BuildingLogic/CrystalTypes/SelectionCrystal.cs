using UnityEngine;

public abstract class SelectionCrystal : Crystal
{
    private SelectionManager _selectionManager;
    [SerializeField] private Transform _floatingPart;

    public abstract SelectionOptionContainer GetOptionContainer();
    public Transform FloatingPart => _floatingPart;

    private void Start() => _selectionManager = FindObjectOfType<SelectionManager>();

    public override void Activate()
    {
        StopFromDestroying();

        _selectionManager.StartSelection(this);

        _selectionManager.OptionChoosen.AddListener(ApplySelectionOption);

        _selectionManager.OptionChoosen.AddListener(UseCrystal);
    }

    protected abstract void ApplySelectionOption(SelectionOption selectionOption);

    private void UseCrystal(SelectionOption option)
    {
        GetOptionContainer().TryToRemoveSelectionOption(option);

        _selectionManager.OptionChoosen.RemoveListener(ApplySelectionOption);
        _selectionManager.OptionChoosen.RemoveListener(UseCrystal);

        CrystalUsed.Invoke();
    }
}
