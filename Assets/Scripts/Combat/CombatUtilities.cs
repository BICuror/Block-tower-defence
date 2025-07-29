using UnityEngine;

public static class CombatUtilities
{
    private const float ISLOATION_CUBE_RADIUS = 1f;

    public static bool EntityIsIsolated(GameObject gameObject)
    {
        return Physics.OverlapSphere(gameObject.transform.position, ISLOATION_CUBE_RADIUS, gameObject.layer).Length <= 1;
    }
}
