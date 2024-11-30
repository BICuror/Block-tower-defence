using UnityEngine;

[CreateAssetMenu(fileName = "DefaultPlacementModule", menuName = "PlacementConditions/DefaultPlacementModule")]

public sealed class DefaultPlacementModule : PlacementModule
{
    [SerializeField] private LayerSetting _sutableTerrainLayerSetting;
    [SerializeField] private LayerSetting _nonStackableLayerSetting;

    public override bool CanBePlaced(GameObject objectToPlace, int x, int z)
    {
        if (!Physics.Raycast(new Vector3(x, 100000f, z), Vector3.down, Mathf.Infinity, _sutableTerrainLayerSetting.GetLayerMask())) return false; 

        RaycastHit[] nonStackableHits = Physics.RaycastAll(new Vector3(x, 100000f, z), Vector3.down, Mathf.Infinity, _nonStackableLayerSetting.GetLayerMask());

        if (nonStackableHits.Length == 0) return true;
        if (nonStackableHits.Length == 1 && nonStackableHits[0].collider.gameObject == objectToPlace) return true;
        
        return false;
    }

    public override float GetHeight(int x, int z)
    {
        Ray ray = new Ray(new Vector3(x, 10000f, z), Vector3.down);

        Physics.Raycast(ray, out RaycastHit terrainHit, Mathf.Infinity, _sutableTerrainLayerSetting.GetLayerMask());
        
        return terrainHit.point.y;
    }

    public override Vector2Int GetPlacementPosition(int x, int z)
    {
        return new Vector2Int(x, z);
    }
}