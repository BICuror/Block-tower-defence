using UnityEngine;

[CreateAssetMenu(fileName = "BuildTime", menuName = "Parameter/BuildTime")]

public sealed class BuildTimeParameter : Parameter
{
    public override string GetValue(GameObject inspectable)
    {
        return inspectable.GetComponent<Building>().BuildTime.ToString();
    }
}
