using UnityEngine;

public class OfferContainer : MonoBehaviour
{
    private DraggableObject _childBuilding;

    private bool _wasDraggable;

    private void Start()
    {
        _childBuilding = transform.GetChild(0).GetComponent<DraggableObject>();

        _childBuilding.PickUp();

        _wasDraggable = _childBuilding.IsDraggable();

        _childBuilding.SetDraggableState(false);
    }

    private void OnCollisionEnter(Collision other) 
    {    
        transform.GetChild(0).GetComponent<DraggableObject>().SetDraggableState(_wasDraggable);

        GameObject mainObject = gameObject.transform.GetChild(0).gameObject;

        mainObject.transform.SetParent(null);

        mainObject.transform.rotation = Quaternion.identity;
    
        mainObject.transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));

        Destroy(gameObject);

        _childBuilding.Place();           
    }
}
