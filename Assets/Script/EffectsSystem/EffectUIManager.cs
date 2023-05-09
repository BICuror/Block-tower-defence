using System.Collections.Generic;
using UnityEngine;

public class EffectUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _effectsUIPrefab;

    [SerializeField] private float _maxDistanceFromCenter;

    private List<EffectPanel> _effectsUI; 

    private void Awake() 
    {
        _effectsUI = new List<EffectPanel>();   
    }

    public void AddEffectUI(Effect effectToAdd)
    {
        GameObject newEffectUI = Instantiate(_effectsUIPrefab, Vector3.zero, transform.rotation, transform);

        EffectPanel panel = newEffectUI.GetComponent<EffectPanel>();

        panel.SetEffect(effectToAdd);

        _effectsUI.Add(panel);

        IntrepolateEffectUIPositions();
    }   

    public void RemoveEffectUI(Effect effectToRemove)
    {
        for (int i = 0; i < _effectsUI.Count; i++)
        {
            if (_effectsUI[i].CompareEffect(effectToRemove))
            {
                Destroy(_effectsUI[i].gameObject);

                _effectsUI.RemoveAt(i);

                break;
            }
        }

        IntrepolateEffectUIPositions();
    }

    private void IntrepolateEffectUIPositions()
    {
        float step = _maxDistanceFromCenter * 2 / (_effectsUI.Count + 1);

        for (int i = 0; i < _effectsUI.Count; i++)
        {
            _effectsUI[i].transform.localPosition = new Vector3(step * (i + 1) - _maxDistanceFromCenter, 0f, 0f);
        }
    }
}
