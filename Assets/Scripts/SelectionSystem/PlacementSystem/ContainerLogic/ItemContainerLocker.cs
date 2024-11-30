using UnityEngine;

public sealed class ItemContainerLocker : MonoBehaviour
{
    [SerializeField] private ItemsContainer _itemsContainer;
    [SerializeField] private GameObject _itemPlacementPoint;

    private bool _possibleToAddItems;
    private bool _possibleToRemoveItems;

    public bool PossibleToRemoveItems => _possibleToRemoveItems;

    public void SetPossibleToAddItems(bool state)
    {
        _possibleToAddItems = state;
        UpdateItemDetectorState();
    }

    public void SetPossibleToRemoveItems(bool state)
    {
        _possibleToRemoveItems = state;
        UpdateItemsDraggableState();
    }

    private void UpdateItemsDraggableState()
    {
        foreach (Item item in _itemsContainer.ContainedItems)
        {
            item.SetDraggableState(_possibleToRemoveItems);
            Debug.Log($"Set draggable state: {item.gameObject.name}, {_possibleToRemoveItems}");
        }
    }

    private void UpdateItemDetectorState()
    {
        _itemPlacementPoint.SetActive(_possibleToAddItems);
    }
}