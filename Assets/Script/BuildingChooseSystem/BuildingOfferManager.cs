using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class BuildingOfferManager : MonoBehaviour
{
    public UnityEvent<BuildingOffer> BuildingChoosen;

    [SerializeField] private BuildingOfferPanel[] _offerPanels;

    [SerializeField] private BuildingAssortiment _buildingAssortiment;

    private void Start()
    {
        StartOffer();

        for (int i = 0; i < _offerPanels.Length; i++)
        {
            _offerPanels[i].OptionChosen.AddListener(BuildingChosen);
        }
    }

    public void StartOffer()
    {
        BuildingChoiseData[] buildings = GetRandomBuildings();

        for (int i = 0; i < buildings.Length; i++)
        {
            _offerPanels[i].SetBuildingChoiseData(buildings[i]);
        }
    }

    public void StopOffer()
    {
        SetOfferState(false);
    }

    private BuildingChoiseData[] GetRandomBuildings()
    {
        List<BuildingChoiseData> possibleBuildings = new List<BuildingChoiseData>(_buildingAssortiment.Buildings);

        BuildingChoiseData[] resultBuildings = new BuildingChoiseData[_offerPanels.Length];

        for (int i = 0; i < resultBuildings.Length; i++)
        {
            int randomIndex = Random.Range(0, possibleBuildings.Count);

            resultBuildings[i] = possibleBuildings[randomIndex];

            possibleBuildings.RemoveAt(randomIndex);
        }

        return resultBuildings;
    }

    private void BuildingChosen(BuildingOffer buildingOffer)
    {
        BuildingChoosen?.Invoke(buildingOffer);
    }

    public void SetOfferState(bool state)
    {
        for (int i = 0; i < _offerPanels.Length; i++)
        {
            _offerPanels[i].gameObject.SetActive(state);
        }
    }
}
