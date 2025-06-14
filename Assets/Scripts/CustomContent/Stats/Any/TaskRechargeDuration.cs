public sealed class TaskRechargeDuration : Stat
{
    protected override float MinimalValue => 0.05f;
    public override bool LowValueIsGood => true;
}