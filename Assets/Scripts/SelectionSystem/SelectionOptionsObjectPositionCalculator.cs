using UnityEngine;

public sealed class SelectionOptionsObjectsPositionCalculator : MonoBehaviour
{
    [SerializeField] private float _maxX;
    [SerializeField] private AnimationCurve _heightCurve;
    [SerializeField] private float _heightMultiplyer;

    public Vector3[] GetPanelsPosition(int positionsAmount)
    {
        Vector3[] positions = new Vector3[positionsAmount];

        float horizontalRange = _maxX * 2;

        float horizontalStep = horizontalRange / (positionsAmount + 1);

        for (int i = 0; i < positionsAmount; i++)
        {
            float x = horizontalStep * (i + 1) - _maxX;
            float z = -_heightCurve.Evaluate(Mathf.Abs(x) / _maxX) * _heightMultiplyer;

            positions[i] = new Vector3(x, 0f, z);
        }

        return positions;
    }
}
