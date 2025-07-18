public sealed class ExplotionDelay : Stat
{
    protected override float MinimalValue => 0.1f;
    public override bool LowValueIsGood => true;
}