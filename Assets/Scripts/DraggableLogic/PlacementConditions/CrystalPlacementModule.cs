using UnityEngine;

[CreateAssetMenu(fileName = "CrystalPlacementModule", menuName = "PlacementConditions/CrystalPlacementModule")]

public sealed class CrystalPlacementModule : PlacementModule
{
    [SerializeField] private LayerSetting _sutableTerrainLayerSetting;
    [SerializeField] private LayerSetting _nonStackableLayerSetting;
    [SerializeField] private LayerSetting _townhallLayerSetting;

    public override bool CanBePlaced(GameObject objectToPlace, int x, int z)
    {
        if (!Physics.Raycast(new Vector3(x, 100000f, z), Vector3.down, Mathf.Infinity, _sutableTerrainLayerSetting.GetLayerMask())) return false;

        RaycastHit[] nonStackableHits = Physics.RaycastAll(new Vector3(x, 100000f, z), Vector3.down, Mathf.Infinity, _nonStackableLayerSetting.GetLayerMask());

        if (nonStackableHits.Length == 0) return true;
        if (nonStackableHits.Length == 1 && nonStackableHits[0].collider.gameObject == objectToPlace) return true;
        
        return Physics.Raycast(new Vector3(x, 10000f, z), Vector3.down, Mathf.Infinity, _townhallLayerSetting.GetLayerMask());
    }

    public override float GetHeight(int x, int z)
    {
        Ray ray = new Ray(new Vector3(x, 10000f, z), Vector3.down);
        
        if (Physics.Raycast(ray, out RaycastHit townhallHit, Mathf.Infinity, _townhallLayerSetting.GetLayerMask()))
        {
            return townhallHit.point.y;
        }

        Physics.Raycast(ray, out RaycastHit terrainHit, Mathf.Infinity, _sutableTerrainLayerSetting.GetLayerMask());
        
        return terrainHit.point.y;
    }
    
    public override Vector2Int GetPlacementPosition(int x, int z)
    {
        Ray ray = new Ray(new Vector3(x, 10000f, z), Vector3.down);
        
        if (Physics.Raycast(ray, out RaycastHit townhallHit, Mathf.Infinity, _townhallLayerSetting.GetLayerMask()))
        {
            return new Vector2Int(Mathf.RoundToInt(townhallHit.transform.position.x), Mathf.RoundToInt(townhallHit.transform.position.z));
        }

        return new Vector2Int(x, z);
    }
}