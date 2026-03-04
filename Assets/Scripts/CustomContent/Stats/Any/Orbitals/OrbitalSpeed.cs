public sealed class OrbitalSpeed : Stat
{
    protected override float MinimalValue => 0.01f;
    public override bool LowValueIsGood => true;
}