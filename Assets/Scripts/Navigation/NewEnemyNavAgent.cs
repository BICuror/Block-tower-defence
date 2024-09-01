using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zenject;
using Navigation;

public class NewEnemyNavAgent : DraggableObject
{
    [SerializeField] private float _timePerBlock = 1f;
    [SerializeField] private AnimationCurve _sameHeightCurve;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private AnimationCurve _differentHeightCurve;
    [SerializeField] private AnimationCurve _rotationCurve;

    [Inject] private EnemyNavigator _enemyNavigator;

    private Vector2Int _previousRotation;

    private NavigationNode _nextNavigationNode;
    private NavigationNode _nextNextNavigationNode;

    private void Start()
    {
        PickedUp.AddListener(StopAllCoroutines);
        Placed.AddListener(OnEnable);
    }

    private void OnEnable()
    {
        _nextNavigationNode = EnemyNavigator.Instance.GetNavigationNode(transform.position);
        _nextNextNavigationNode = _nextNavigationNode.GetNextNode();

        Vector2 previousNodePosition = new Vector2(_nextNavigationNode.Position.x, _nextNavigationNode.Position.z);
        Vector2 currentNodePosition = new Vector2(_nextNextNavigationNode.Position.x, _nextNextNavigationNode.Position.z);

        _previousRotation = new Vector2Int(Mathf.RoundToInt(currentNodePosition.x - previousNodePosition.x), Mathf.RoundToInt(currentNodePosition.y - previousNodePosition.y));
        
        transform.LookAt(new Vector3(transform.position.x + _previousRotation.x, transform.position.y, transform.position.z + _previousRotation.y));

        StartCoroutine(MoveTowardsNode());
    }

    private IEnumerator MoveTowardsNode()
    {
        NavigationNode currentNode = _nextNextNavigationNode;
        NavigationNode nextNode = _nextNextNavigationNode.GetNextNode();

        Vector2 previousNodePosition = new Vector2(_nextNavigationNode.Position.x, _nextNavigationNode.Position.z);
        Vector2 currentNodePosition = new Vector2(currentNode.Position.x, currentNode.Position.z);
        Vector2 nextNodePosition = new Vector2(nextNode.Position.x, nextNode.Position.z);

        Vector2Int newRotation = new Vector2Int(Mathf.RoundToInt(nextNodePosition.x - currentNodePosition.x), Mathf.RoundToInt(nextNodePosition.y - currentNodePosition.y));

        bool willRotate = _previousRotation != newRotation;

        Vector2Int additionalVector = new Vector2Int(0, 0);

        if (willRotate)
        {
            if ((_previousRotation + newRotation).magnitude == 0f)
            {
                Debug.Log("will rotate otherr");
                if (newRotation.x != 0)
                {
                    if (Random.Range(0, 100) > 50)
                    {
                        additionalVector = new Vector2Int(0, 1);
                    }
                    else 
                    {
                        additionalVector = new Vector2Int(0, -1);
                    }
                }
                else
                {
                    if (Random.Range(0, 100) > 50)
                    {
                        additionalVector = new Vector2Int(-1, 0);
                    }
                    else 
                    {
                        additionalVector = new Vector2Int(1, 0);
                    }
                }
            }
        }

        AnimationCurve heightCurve = _sameHeightCurve;

        if (_nextNavigationNode.Position.y != currentNode.Position.y) heightCurve = _differentHeightCurve;

        float elapsedTime = 0f;
        while (elapsedTime < _timePerBlock)
        {
            elapsedTime += Time.deltaTime;

            float height = heightCurve.Evaluate(elapsedTime) * _jumpHeight;

            Vector3 enemyPosition = Vector3.Lerp(_nextNavigationNode.Position, currentNode.Position, elapsedTime / _timePerBlock);
            enemyPosition.y += height;

            transform.position = enemyPosition;
            
            if (willRotate)
            {
                Vector2 interpolatedPosition = Vector2.Lerp(_previousRotation, newRotation, elapsedTime / _timePerBlock);

                interpolatedPosition = interpolatedPosition + (additionalVector * new Vector2(_rotationCurve.Evaluate(elapsedTime / _timePerBlock), _rotationCurve.Evaluate(elapsedTime / _timePerBlock)));

                transform.LookAt(new Vector3(transform.position.x + interpolatedPosition.x, transform.position.y, transform.position.z + interpolatedPosition.y));
            }

            yield return new WaitForFixedUpdate();
        } 

        _previousRotation = newRotation;

        _nextNavigationNode = currentNode;
        _nextNextNavigationNode = nextNode;
        
        StartCoroutine(MoveTowardsNode());
    }
}
