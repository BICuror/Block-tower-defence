using UnityEngine;

[CreateAssetMenu(fileName = "RechargeTime", menuName = "Parameter/RechargeTime")]

public sealed class RechargeTimeParameter : Parameter
{
    public override string GetValue(GameObject inspectable)
    {
        return inspectable.GetComponent<TaskCycle>().RechargeTime.ToString();
    }
}
