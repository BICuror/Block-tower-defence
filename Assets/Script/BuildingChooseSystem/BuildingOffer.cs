using UnityEngine;

public struct BuildingOffer
{
    private GameObject _buildingPrefab;
    private int _buildingAmount;

    public BuildingOffer(GameObject buildingPrefab, int amount)
    {
        _buildingPrefab = buildingPrefab;
        _buildingAmount = amount;
    }  

    public int GetBuildingAmount() => _buildingAmount;
    public GameObject GetBuildingPrefab() => _buildingPrefab;  
}

