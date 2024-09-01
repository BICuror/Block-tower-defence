using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public sealed class SelectionOptionObject : MonoBehaviour, IActivatable
{
    [SerializeField] private VisualEffect _visualEffect;

    [SerializeField] private InspectableObject _inspectableObject;

    public UnityEvent<SelectionOption> Choosen;

    private SelectionOption _selectionOption;

    public void SetSelectionOption(SelectionOption option) 
    {
        _selectionOption = option;

        _inspectableObject.SetName(option.Name);
        _inspectableObject.SetDescription(option.Description);

        if (option is BuildingSelectionOption)
        {
            BuildingSelectionOption bso = option as BuildingSelectionOption;

            DraggableObject building = Instantiate(bso.Building, transform.position, transform.rotation, transform);
            building.transform.localPosition = new Vector3(0f, 0.4f, -0.4f);
            building.transform.Rotate(-10f, 45f, -10f);
            building.PickUp();

            for (int i = 0; i < building.transform.childCount; i++)
            {
                if (building.transform.GetChild(i).gameObject.TryGetComponent<AreaDetectorBase>(out AreaDetectorBase scaner))
                {
                    Destroy(scaner);
                    break;
                }
            }

            Destroy(building.GetComponent<Rigidbody>());

            building.SetDraggableState(false);
        }
        else 
        {
            UpgradeSelectionOption uso = option as UpgradeSelectionOption;

            Instantiate(uso.Inspectable, transform.position, Quaternion.identity, transform);
        }
        
    }

    public void DisableEffects() => _visualEffect.Stop();

    public void Activate() => Choosen.Invoke(_selectionOption);
}
