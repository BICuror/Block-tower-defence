using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public abstract class SelectorBase<T> : MonoBehaviour where T : SelectionOptionObject
{
    [Inject] private GlobalStatContainer _globalStatContainer;
    
    [SerializeField] private SelectionOptionObjectController _selectionOptionObjectController;
    [SerializeField] private T _selectionObject;
    
    public abstract UniTask StartSelection(SelectionSettings settings);
    
    protected async UniTask CreateOptionObjects(SelectionSettings settings)
    {
        settings.SelectionOptionsAmount = GetSelectionOptionsAmount(settings);
        
        await _selectionOptionObjectController.CreateSelectionOptionObjects(_selectionObject, settings.SelectionOptionsAmount, InitializeSelectionOption);
    }

    protected int GetSelectionOptionsAmount(SelectionSettings settings)
    {
        int optionObjectsAmount = _globalStatContainer.Get<SelectionOptionsAmount>().RoundedValue;
        
        if (settings.SelectionOptionsAmount > 0) optionObjectsAmount = settings.SelectionOptionsAmount;
        
        return optionObjectsAmount;
    }
    
    protected abstract void InitializeSelectionOption(T optionObject);
}
