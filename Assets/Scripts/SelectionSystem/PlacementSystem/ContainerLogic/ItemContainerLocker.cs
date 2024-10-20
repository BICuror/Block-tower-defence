using UnityEngine;

public sealed class ItemContainerLocker : MonoBehaviour
{
    [SerializeField] private ItemsContainer _itemsContainer;
    [SerializeField] private ItemDetector _itemDetector;

    private bool _possibleToAddItems;
    private bool _possibleToRemoveItems;

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
        }
    }

    private void UpdateItemDetectorState()
    {
        _itemDetector.gameObject.SetActive(_possibleToAddItems);
    }
}