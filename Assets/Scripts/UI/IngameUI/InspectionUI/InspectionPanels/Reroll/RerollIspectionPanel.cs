using CuroLocalization;
using UnityEngine;
using Zenject;
using TMPro;

public sealed class RerollIspectionPanel : InspectionPanelBase
{
    [Inject] private SelectionManager _selectionManager;

    [SerializeField] private TextMeshProUGUI _leftChargesLeftTextField;
    [SerializeField] private string _chargesLeftLocKey;
    
    public void Initialize(Transform target)
    {
        InitializeInspectionPanelBase(target.GetComponent<InspectableObject>());
        
        _leftChargesLeftTextField.text = _chargesLeftLocKey.Localize().Replace("{rerollChargesLeft}", _selectionManager.RerollsLeft.ToString());
    }
}