using UnityEngine;

[CreateAssetMenu(fileName = "CrystalPlacementCondition", menuName = "PlacementConditions/CrystalPlacementCondition")]

public class CrystalPlacementCondition : PlacementCondition
{
    [SerializeField] private LayerSetting _sutableTerrainLayerSetting;
    [SerializeField] private LayerSetting _nonStackableLayerSetting;
    [SerializeField] private LayerSetting _townhallLayerSetting;

    public override bool IsSatisfied(GameObject objectToPlace, int x, int y)
    {
        RaycastHit[] terrainHits = Physics.RaycastAll(new Vector3(x, 100000f, y), Vector3.down, Mathf.Infinity, _sutableTerrainLayerSetting.GetLayerMask());

        if (terrainHits.Length == 0) return false; 

        RaycastHit[] nonStackableHits = Physics.RaycastAll(new Vector3(x, 100000f, y), Vector3.down, Mathf.Infinity, _nonStackableLayerSetting.GetLayerMask());

        if (nonStackableHits.Length == 0) return true;
        else if (nonStackableHits.Length == 1 && nonStackableHits[0].collider.gameObject == objectToPlace) return true;
        else if (nonStackableHits.Length >= 1)
        {
            return Physics.Raycast(new Vector3(x, 10000f, y), Vector3.down, Mathf.Infinity, _townhallLayerSetting.GetLayerMask());
        }
        else return false;
    }
}
