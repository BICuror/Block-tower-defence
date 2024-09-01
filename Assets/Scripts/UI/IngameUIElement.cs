using UnityEngine;

public abstract class IngameUIElement : MonoBehaviour
{
    public void LookAt(Vector3 position)
    {
        transform.LookAt(position);

        transform.Rotate(90f, 0f, 0f);
    }
}
