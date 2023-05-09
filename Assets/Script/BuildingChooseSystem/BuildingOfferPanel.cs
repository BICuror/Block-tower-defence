using UnityEngine;
using UnityEngine.Events;

public class BuildingOfferPanel : MonoBehaviour, IActivatable
{
    private BuildingChoiseData _buildingChoiseData;

    public UnityEvent<BuildingOffer> OptionChosen;

    [SerializeField] private Material _material;

    public void SetBuildingChoiseData(BuildingChoiseData buildingChoiseData)
    {
        _buildingChoiseData = buildingChoiseData;
    
        SetImage();
    }

    private void SetImage()
    {
        Material materialToSet = new Material(_material);

        materialToSet.SetTexture("_Texture", _buildingChoiseData.BuildingIcon); 

        GetComponent<MeshRenderer>().material = materialToSet;
    }

    public void Activate()
    {
        OptionChosen.Invoke(CreateBuildingOffer());
    }

    private BuildingOffer CreateBuildingOffer()
    {
        return new BuildingOffer(_buildingChoiseData.BuildingPrefab, 1);
    }
}
