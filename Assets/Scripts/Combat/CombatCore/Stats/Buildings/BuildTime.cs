public sealed class BuildTime : Stat
{
    protected override float MinimalValue { get => 0.05f; }
    public override bool LowValueIsGood => true;
}