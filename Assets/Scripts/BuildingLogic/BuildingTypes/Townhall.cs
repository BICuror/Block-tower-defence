using UnityEngine;

namespace Combat
{
    public sealed class Townhall : MonoBehaviour
    {
        public void SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition + Vector3.up;
        }
    }
}