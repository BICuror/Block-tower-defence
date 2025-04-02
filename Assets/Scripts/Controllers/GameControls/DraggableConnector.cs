using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;
using DG.Tweening;

public sealed class DraggableConnector : MonoBehaviour
{
    [Header("JointSettings")]
    [SerializeField] private Joint _joint;
    [SerializeField] private float _dragSpeed;

    [Header("PlacementSettings")]
    public UnityEvent<GameObject> PlacedDraggable;
    [SerializeField] private float _placementDuration;
    
    public async UniTask PlaceDraggable(GameObject draggableObject, IDraggable draggable, Vector3 finalPosition)
    {
        DragAnimationObject dragAnimationObject = draggable.GetDragAnimationObject();
        
        dragAnimationObject.DisconnectFromJoint(_joint);
        
        MoveToFinalPosition(finalPosition);
        PlaceObject(dragAnimationObject, finalPosition);

        await UniTask.WaitForSeconds(_placementDuration);
        await UniTask.WaitForFixedUpdate();
        
        draggableObject.transform.parent = null;
        draggableObject.transform.position = finalPosition; 
        draggable.Place();
        
        dragAnimationObject.SetInitialParent();

        PlacedDraggable.Invoke(draggableObject);
    }

    private async UniTask PlaceObject(DragAnimationObject dragAnimationObject, Vector3 finalPosition)
    {
        finalPosition += dragAnimationObject.InitialLocalPosition;
        
        Vector3 initialPosition = dragAnimationObject.transform.position;

        Vector3 initialRotation = dragAnimationObject.transform.rotation.eulerAngles;

        Vector3 finalRotation = new Vector3(0f, GetFinalYRotation(dragAnimationObject.transform.rotation.eulerAngles.y), 0f);

        await DOVirtual.Float(0f, 1f, _placementDuration, Evaluate).AsyncWaitForCompletion();

        dragAnimationObject.transform.position = finalPosition;
        dragAnimationObject.transform.rotation = Quaternion.Euler(finalRotation);

        void Evaluate(float value)
        {
            float evaluatedX;
            if (initialRotation.x > 180) evaluatedX = Mathf.Lerp(initialRotation.x, 360f, value);
            else evaluatedX = Mathf.Lerp(initialRotation.x, 0, value);
            
            float evaluatedY = Mathf.Lerp(initialRotation.y, finalRotation.y, value); 
            
            float evaluatedZ; 
            if (initialRotation.z > 180) evaluatedZ = Mathf.Lerp(initialRotation.z, 360f, value);
            else evaluatedZ = Mathf.Lerp(initialRotation.z, 0, value);
            
            dragAnimationObject.transform.position = Vector3.Lerp(initialPosition, finalPosition, value);
            dragAnimationObject.transform.rotation = Quaternion.Euler(new Vector3(evaluatedX, evaluatedY, evaluatedZ));
        }
    }

    private async UniTask MoveToFinalPosition(Vector3 finalPosition)
    {
        await DOVirtual.Float(0f, 1f, _placementDuration, (value) => MoveTowardsPosition(finalPosition)).AsyncWaitForCompletion();
    }
    
    private float GetFinalYRotation(float currentRotation)
    {
        if (currentRotation >= 45 && currentRotation < 135) return 90f;
        if (currentRotation >= 135 && currentRotation < 225) return 180f;
        if (currentRotation >= 225 && currentRotation < 315) return 270f;
        if (currentRotation >= 315 && currentRotation <= 360) return 360f; 
        return 0f;
    }
    
    public void PickUpDraggable(GameObject draggableObject)
    {
        IDraggable draggable = draggableObject.GetComponent<IDraggable>();
        
        draggable.GetDragAnimationObject().ConnectToJoint(_joint);
        
        draggableObject.transform.SetParent(transform); 
        draggableObject.transform.localPosition = Vector3.zero;
    }

    public void MoveTowardsPosition(Vector3 position)
    {
        float distance = Vector3.Distance(position, transform.position);

        transform.position = Vector3.MoveTowards(transform.position, position, _dragSpeed * distance);   
    }
}