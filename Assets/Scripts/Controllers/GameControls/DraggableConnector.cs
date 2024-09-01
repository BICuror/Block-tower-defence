using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public sealed class DraggableConnector : MonoBehaviour
{
    [Header("JointSettings")]

    [SerializeField] private Joint _joint;

    [SerializeField] private Vector3 _distance;

    [Header("PlacementSettings")]

    public UnityEvent<GameObject> PlacedDraggable;

    [SerializeField] private int _placementFramesDuration;

    private Coroutine _transformCoroutine;

    private Vector3 rotStep;

    public void StartPlacementAnimation(GameObject objectToPlace, Vector3 finalPosition)
    {
        StartCoroutine(PlaceObject(objectToPlace, finalPosition));

        _transformCoroutine = StartCoroutine(TransformMoving(finalPosition));
    }

    private IEnumerator PlaceObject(GameObject objectToPlace, Vector3 finalPosition)
    {
        YieldInstruction instruction = new WaitForFixedUpdate();

        Vector3 initialPosition = objectToPlace.transform.position;

        Vector3 initialRotation = objectToPlace.transform.rotation.eulerAngles;

        Vector3 finalRotation = new Vector3(0f, GetFinalYRotation(objectToPlace.transform.rotation.eulerAngles.y), 0f);

        for(float i = 0; i <= _placementFramesDuration; i++)
        {
            yield return instruction;

            float evaluatedValue = i / (float)_placementFramesDuration; 
            
            float evaluatedX;
            if (initialRotation.x > 180) evaluatedX = Mathf.Lerp(initialRotation.x, 360f, evaluatedValue);
            else evaluatedX = Mathf.Lerp(initialRotation.x, 0, evaluatedValue);

            float evaluatedY = Mathf.Lerp(initialRotation.y, finalRotation.y, evaluatedValue);
            float evaluatedZ;
            if (initialRotation.z > 180) evaluatedZ = Mathf.Lerp(initialRotation.z, 360f, evaluatedValue);
            else evaluatedZ = Mathf.Lerp(initialRotation.z, 0, evaluatedValue);

            objectToPlace.transform.position = Vector3.Lerp(initialPosition, finalPosition, evaluatedValue);

            objectToPlace.transform.rotation = Quaternion.Euler(new Vector3(evaluatedX, evaluatedY, evaluatedZ));
        }

        PlaceDraggable(objectToPlace);
    }

    public void StopMovingCoroutine()
    {
        if (_transformCoroutine != null) StopCoroutine(_transformCoroutine);
    }

    private IEnumerator TransformMoving(Vector3 finalPosition)
    {
        YieldInstruction instruction = new WaitForFixedUpdate();

        Vector3 connectorInitialPosition = transform.position;

        for(float i = 0; i <= _placementFramesDuration; i++)
        {
            yield return instruction;

            float evaluatedValue = i / (float)_placementFramesDuration; 

            transform.position = Vector3.Lerp(connectorInitialPosition, finalPosition, evaluatedValue);
        }
    }

    private float GetFinalYRotation(float currentRotation)
    {
        if (currentRotation >= 45 && currentRotation < 135) return 90f;
        if (currentRotation >= 135 && currentRotation < 225) return 180f;
        if (currentRotation >= 225 && currentRotation < 315) return 270f;
        if (currentRotation >= 315 && currentRotation <= 360) return 360f;
        else return 0f;
    }

    private void PlaceDraggable(GameObject draggable)
    {
        draggable.GetComponent<IDraggable>().Place();

        PlacedDraggable.Invoke(draggable);
    }

    public void ConnectDraggable(GameObject draggable)
    {
        draggable.transform.position = _joint.transform.position - _distance;

        Rigidbody rigidbody = draggable.GetComponent<Rigidbody>();

        rigidbody.useGravity = true;

        rigidbody.constraints = RigidbodyConstraints.None; 

        _joint.connectedBody = rigidbody;
    } 

    public void DisconnectDraggable(GameObject draggable)
    {
        Rigidbody rigidbody = draggable.GetComponent<Rigidbody>();

        rigidbody.useGravity = false;

        rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;;

        _joint.connectedBody = null;
    } 
}
