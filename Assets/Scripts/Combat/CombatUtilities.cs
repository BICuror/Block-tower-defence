using UnityEngine;

public static class CombatUtilities
{
    private const float _isolationRadius = 1f;

    public static bool EntityIsIsolated(GameObject gameObject)
    {
        return Physics.OverlapSphere(gameObject.transform.position, _isolationRadius, gameObject.layer).Length <= 1;
    }
}
