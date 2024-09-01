using UnityEngine;
using System.Collections.Generic;

public sealed class SeclectionLinesCreator : MonoBehaviour
{
    [SerializeField] private OptionLine _optionLinePrefab;
    [SerializeField] private OptionLine _upgradeOptionLine;

    private OptionLine[] _optionLines;

    public void CreateLines(Transform crystal, Transform[] options, bool buildings)
    {
        _optionLines = new OptionLine[options.Length];

        for (int i = 0; i < _optionLines.Length; i++)
        {
            if (buildings)
            {
                _optionLines[i] = Instantiate(_optionLinePrefab, crystal.position, Quaternion.identity);
            }
            else 
            {
                _optionLines[i] = Instantiate(_upgradeOptionLine, crystal.position, Quaternion.identity); 
            }
            
            _optionLines[i].SetTargets(crystal, options[i]);
        }
    } 

    public void DestroyLines()
    {
        for (int i = 0; i < _optionLines.Length; i++)
        {
            Destroy(_optionLines[i].gameObject);
        }

        _optionLines = new OptionLine[0];
    }
}
