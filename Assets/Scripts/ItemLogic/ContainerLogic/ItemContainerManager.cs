using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ItemContainerManager : MonoBehaviour
{
    [Inject] private WaveIndexContainer _waveIndexContainer;
    [Inject] private SelectionManager _selectionManager;
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

    public async UniTask UpdateContainedItems()
    {
        List<Item> items = new List<Item>(_itemContainer.ContainedItems);

        for (int i = 0; i < items.Count; i++)
        {
            await items[i].DecreaseDuration();
        }
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
