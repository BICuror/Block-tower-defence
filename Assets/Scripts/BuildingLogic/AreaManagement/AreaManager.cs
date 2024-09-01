using UnityEngine;

public class AreaManager : MonoBehaviour
{
    [Header("AreaSettings")]
    [SerializeField] protected float _height = 100f;
    [SerializeField] protected int _radius;
    [SerializeField] private GameObject _reachAreaCollider;

    private void Awake() => UpdateScale();

    public virtual void SetRadius(int value)
    {
        _radius = value;

        UpdateScale();
    }

    public virtual int GetRadius() => _radius;

    public virtual void UpdateScale()
    {
        _reachAreaCollider.transform.localScale = GetScale();
    }

    public virtual Vector3 GetScale()
    {
        float scale = _radius * 2f + 0.95f;

        return new Vector3(scale, _height, scale);
    }
}
