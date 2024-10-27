using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ItemContainerManager : MonoBehaviour
{
    [Inject] private WaveManager _waveManager;
    [SerializeField] private float _transitionDuration = 0.6f;
    [SerializeField] private ItemsContainer _itemContainer;
    [SerializeField] private ItemsContainerAnimator _itemsContainerAnimator;
    [SerializeField] private ItemContainerLocker _itemContainerLocker;

    private void Awake()
    {
        _itemContainer.ContainerUpdated += UpdateContainer;
    }

    public void UnlockContainer()
    {
        _itemContainerLocker.SetPossibleToRemoveItems(true);
        _itemContainerLocker.SetPossibleToAddItems(true);
    }

    private void UpdateContainer()
    {
        StopAllCoroutines();
        
        _itemsContainerAnimator.TransitionToNewPositions(_itemContainer.ContainedItems, _transitionDuration);
        _itemContainerLocker.SetPossibleToRemoveItems(false);

        StartCoroutine(WaitToEnableDraggable());
    }   

    private IEnumerator WaitToEnableDraggable()
    {
        yield return new WaitForSeconds(_transitionDuration);
        _itemContainerLocker.SetPossibleToRemoveItems(true);
    }

    public void LockContainer()
    {
        StopAllCoroutines();
        _itemContainerLocker.SetPossibleToRemoveItems(false);
        _itemContainerLocker.SetPossibleToAddItems(false);
    }
}
