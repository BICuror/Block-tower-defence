using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ItemContainerManager : MonoBehaviour
{
    [Inject] private WaveManager _waveManager;
    [SerializeField] private ItemsContainer _itemContainer;
    [SerializeField] private ItemsContainerAnimator _itemsContainerAnimator;
    [SerializeField] private ItemContainerLocker _itemContainerLocker;

    private void Awake()
    {
        _itemContainer.ContainerUpdated += UpdateContainer;
        _itemContainer.ItemInspectionEnded += StartContainerAnimation;
        _itemContainer.ItemInspectionStarted += StopContainerAnimation;
        
        _itemsContainerAnimator.StartRotation();
    }

    public void UnlockContainer()
    {
        _itemContainerLocker.SetPossibleToAddItems(true);
    }

    private void UpdateContainer()
    {
        StopAllCoroutines();
        _itemsContainerAnimator.TransitionToNewPositions();
    }

    public void UpdateContainedItems()
    {
        List<Item> items = new List<Item>(_itemContainer.ContainedItems);
        
        items.ForEach(item =>
        {
            item.DecreaseDuration();
        });
    }
    
    public void LockContainer()
    {
        StopAllCoroutines();
        _itemContainerLocker.SetPossibleToRemoveItems(false);
        _itemContainerLocker.SetPossibleToAddItems(false);
    }

    private void StartContainerAnimation()
    {
        if (!_itemContainer.IsItemInspected)
        {
            _itemsContainerAnimator.StartRotation();
        }
    }

    private void StopContainerAnimation()
    {
        _itemsContainerAnimator.StopRotation();
    }
}
