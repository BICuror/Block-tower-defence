using UnityEngine;
using Navigation;

public abstract class OptionalTaskGenerator : MonoBehaviour
{
    public abstract bool TryGenerateOptionalTask(Vector2Int spawnerPosition, out AdditionalTaskLayerPrebuildData layerPrebuildData, out OptionalTask optionalTask);
}

public enum OptionalTaskRewardType
{
    UpgradeCharges = 0,
    Reroll = 1,
}