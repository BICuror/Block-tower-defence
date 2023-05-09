using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TawernTower : MonoBehaviour
{
    [SerializeField] private GameObject[] _allTowers;

    [SerializeField] private GameObject _readinessNotification;

    [SerializeField] private GameObject _offerContainer;

    [SerializeField] private BuildingCreator _buildingCreator;

    private bool _readyToGiveTower;

    private GameObject[] _offerTowers;

    private void Start() 
    {
        //SetUpTimeActivatedBuilding();

        //TaskPerformed.AddListener(ActivateTowerOffer);
    
        GetComponent<TaskCycle>().ShouldWorkDelegate = LOL;
    }

    public bool LOL() => true;

   // protected virtual bool ShouldWork() => (_readyToGiveTower == false);

    private void ActivateTowerOffer()
    {
        _readyToGiveTower = true;

        _readinessNotification.SetActive(true);

        GenerateTowerOffer();
    }

    private void GenerateTowerOffer()
    {
        _offerTowers = new GameObject[3];

        for (int i = 0; i < _offerTowers.Length; i++) _offerTowers[i] = _allTowers[Random.Range(0, _allTowers.Length)];
    }

    public void TowerOfferChosen(int chosenOption)
    {
        _readyToGiveTower = false;

        _readinessNotification.SetActive(false);

        CreateTower(chosenOption);

        //Recharge();
    }

    public void CreateTower(int towerIndex)
    {
        _buildingCreator.CreateBuilding(_allTowers[towerIndex]);
    }
}
