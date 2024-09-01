using System.Collections;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(EntityHealth))]

public sealed class HitHighlighter : MonoBehaviour
{
    [SerializeField] private Material _highlightMaterial;
    private Material _defaultMaterial;

    [SerializeField] private MeshRenderer _meshRenderer;

    public UnityEvent Highlited;
    public UnityEvent Unhiglited;

    private float _highlightDuration = 0.1f;
    private YieldInstruction _yieldInstruction;

    private void Awake()
    {
        _defaultMaterial = _meshRenderer.sharedMaterial;

        GetComponent<EntityHealth>().Damaged.AddListener(HiglightEntity);

        _yieldInstruction = new WaitForSeconds(_highlightDuration);

        if (_highlightMaterial == null) Debug.LogError("Highlight material is missing on" + gameObject.name);
    }

    private void HiglightEntity()
    {
        StopAllCoroutines();

        _meshRenderer.sharedMaterial = _highlightMaterial;

        Highlited.Invoke();

        StartCoroutine(UnhighlightEntity());
    }

    private IEnumerator UnhighlightEntity()
    {
        yield return _yieldInstruction;

        _meshRenderer.sharedMaterial = _defaultMaterial;

        Unhiglited.Invoke();
    }
}
