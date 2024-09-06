using UnityEngine;

[CreateAssetMenu(fileName = "DefaultPlacementCondition", menuName = "PlacementConditions/DefaultPlacementCondition")]

public sealed class DefaultPlacementCondition : PlacementCondition
{
    [SerializeField] private LayerSetting _sutableTerrainLayerSetting;
    [SerializeField] private LayerSetting _nonStackableLayerSetting;

    public override bool IsSatisfied(GameObject objectToPlace, int x, int y)
    {
        RaycastHit[] terrainHits = Physics.RaycastAll(new Vector3(x, 100000f, y), Vector3.down, Mathf.Infinity, _sutableTerrainLayerSetting.GetLayerMask());

        if (terrainHits.Length == 0) return false; 

        RaycastHit[] nonStackableHits = Physics.RaycastAll(new Vector3(x, 100000f, y), Vector3.down, Mathf.Infinity, _nonStackableLayerSetting.GetLayerMask());

        if (nonStackableHits.Length == 0) return true;
        else if (nonStackableHits.Length == 1 && nonStackableHits[0].collider.gameObject == objectToPlace) return true;
        else return false;
    }
}