using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingProgressBar : MonoBehaviour
{
    [SerializeField] private Material _progressBarMaterial;

    private Material _currentProgressBarMaterial;

    private void OnEnable() => CreateAndSetNewMaterial();

    private void CreateAndSetNewMaterial()
    {
        _currentProgressBarMaterial = new Material(_progressBarMaterial);

        gameObject.GetComponent<MeshRenderer>().material = _currentProgressBarMaterial;
    }

    public void StartFillingBar(float time)
    {
        gameObject.SetActive(true);

        float step = 1f / time / 50f;

        _currentProgressBarMaterial.SetFloat("BuildValue", 0f);

        StartCoroutine(FillBar(step));
    }

    private IEnumerator FillBar(float step)
    {
        yield return new WaitForFixedUpdate();

        float currentHealth = _currentProgressBarMaterial.GetFloat("BuildValue");

        _currentProgressBarMaterial.SetFloat("BuildValue", currentHealth + step);

        if (currentHealth + step < 1f) StartCoroutine(FillBar(step));
        else gameObject.SetActive(false);
    }

    public void StopFillingBar()
    {
        StopAllCoroutines();

        gameObject.SetActive(false);
    }
}
