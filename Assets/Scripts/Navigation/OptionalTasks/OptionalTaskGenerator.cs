using UnityEngine;
using Navigation;

public abstract class OptionalTaskGenerator : MonoBehaviour
{
    public abstract bool TryGenerateOptionalTask(Vector2Int spawnerPosition, out AdditionalTaskLayerPrebuildData layerPrebuildData);
}