using UnityEngine;
using UnityEngine.AI;

public sealed class EnemyDestanationSetter : MonoBehaviour
{
    private static EnemyDestanationSetter _instance;

    public static EnemyDestanationSetter Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else 
        {
            Debug.LogError("Multiple instances of EnemyDestantionSetter");
        }
    }

    [SerializeField] private Transform _townhall;

    public Vector3 GetFinalEnemyDestanation() => _townhall.position;
}
