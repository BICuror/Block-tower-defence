using UnityEngine;

[CreateAssetMenu(fileName = "ReachArea", menuName = "Parameter/ReachArea")]

public sealed class ReachAreaParameter : Parameter
{
    public override string GetValue(GameObject inspectable)
    {
        return inspectable.GetComponent<AreaManager>().GetRadius().ToString();
    }
}
