using UnityEngine;
using DG.Tweening;

public sealed class SelectionAnimator : MonoBehaviour
{
    [SerializeField] private SeclectionLinesCreator _selectionLineCreator;

    private SelectionOptionObject[] _selectionOptions;

    public void StartSelectionAnimation(Vector3[] positions, SelectionOptionObject[] selectionOptions, SelectionCrystal crystal)
    {
        Transform[] transforms = new Transform[selectionOptions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            selectionOptions[i].transform.DOLocalMove(positions[i], 1f);
            selectionOptions[i].transform.DOScale(1f, 1f);
            
            transforms[i] = selectionOptions[i].transform;
        }

        _selectionLineCreator.CreateLines(crystal.FloatingPart, transforms, !crystal.gameObject.TryGetComponent<UpgradeSelectionCrystal>(out UpgradeSelectionCrystal ow));
    }

    public void StopSelectionAnimation(SelectionOptionObject[] selectionOptions)
    {
        _selectionOptions = selectionOptions;

        for (int i = 0; i < selectionOptions.Length; i++)
        {
            selectionOptions[i].DisableEffects();
            selectionOptions[i].transform.DOLocalMove(Vector3.zero, 1f);
            selectionOptions[i].transform.DOScale(0f, 1f);
        }

        Invoke("DestroyAll", 1.1f);
    }

    private void DestroyAll()
    {
        _selectionLineCreator.DestroyLines();

        for (int i = 0; i < _selectionOptions.Length; i++)
        {   
            Destroy(_selectionOptions[i].gameObject);
        }

        _selectionOptions = new SelectionOptionObject[0];
    }
}
