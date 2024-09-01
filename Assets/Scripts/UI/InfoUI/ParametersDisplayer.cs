using UnityEngine;

public sealed class ParametersDisplayer : MonoBehaviour
{
    [SerializeField] private ParameterPanel[] _panels;

    public void DisplayParameters(BuildingInspectable buildingInpectable)
    {
        Parameter[] parameters = buildingInpectable.GetAllParameters();

        for (int i = 0; i < _panels.Length; i++)
        {
            if (parameters.Length > i)
            {
                _panels[i].SetParameterData(parameters[i], buildingInpectable.gameObject);
                _panels[i].gameObject.SetActive(true);
            }
            else 
            {
                _panels[i].gameObject.SetActive(false);
            }
        }
    }
}
