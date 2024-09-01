using UnityEngine;

public sealed class CustomAreaManager : AreaManager
{
    [SerializeField] private Mesh _customMesh;
    public Mesh GetCustomMesh() => _customMesh; 

    [SerializeField] private int _shownRadius;

    public override void UpdateScale() {}

    public override void SetRadius(int value) {}

    public override int GetRadius() => _shownRadius;

    public override Vector3 GetScale()
    {
        float scale = _radius - 0.05f;

        return new Vector3(scale, _height, scale);
    }
}
