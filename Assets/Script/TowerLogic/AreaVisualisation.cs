using System.Collections;
using UnityEngine;

public sealed class AreaVisualisation : MonoBehaviour
{
    [Header("AreaSettings")]
    [SerializeField] private float _height = 100f;
    [SerializeField] private int _radius;

    [Header("VisualisationSettings")]
    [Range(0f, 1f)] [SerializeField] private float _visualisationSpreadSpeed = 0.08f;

    [Header("Links")]
    [SerializeField] private GameObject _reachAreaVisualisation;
    [SerializeField] private GameObject _reachAreaCollider;

    private void Start()
    {   
        Building building = GetComponent<Building>(); 

        building.PickedUp.AddListener(ActivateVisualisation);
        building.Placed.AddListener(DisactivateVisualisation);
    
        SetScale(_radius * 2f + 0.95f);
    }

    private void SetScale(float scale)
    {
        _reachAreaCollider.transform.localScale = new Vector3(scale, _height, scale);
        _reachAreaVisualisation.transform.localScale = Vector3.zero;
    }

    public void ActivateVisualisation()
    {
        StopAllCoroutines();

        _reachAreaVisualisation.SetActive(true);

        StartCoroutine(VisualisationAppear());
    }
    
    private IEnumerator VisualisationAppear()
    {
        yield return new WaitForFixedUpdate();

        float scale = Mathf.Lerp(_reachAreaVisualisation.transform.localScale.x, _radius * 2f + 0.95f, _visualisationSpreadSpeed);

        _reachAreaVisualisation.transform.localScale = new Vector3(scale, _height, scale);

        StartCoroutine(VisualisationAppear());
    }

    public void DisactivateVisualisation()
    {
        StopAllCoroutines();

        StartCoroutine(VisualisationDisappear());
    }

    private IEnumerator VisualisationDisappear()
    {
        yield return new WaitForFixedUpdate();

        float scale = Mathf.Lerp(_reachAreaVisualisation.transform.localScale.x, 0, _visualisationSpreadSpeed);

        _reachAreaVisualisation.transform.localScale = new Vector3(scale, _height, scale);

        if (scale > 1f) StartCoroutine(VisualisationDisappear());
        else _reachAreaVisualisation.SetActive(false);
    }
}
