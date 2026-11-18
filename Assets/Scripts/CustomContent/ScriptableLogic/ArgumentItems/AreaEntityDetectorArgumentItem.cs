using ScriptableLogic.SubDomains;
using Configs;
using Combat;

namespace ScriptableLogic.ArgumentItem
{
    public sealed class AreaEntityDetectorArgumentItem : ArgumentItem
    {
        [Argument("UseCustomArea")] private bool _useCustomArea;

        public AreaEntityDetector GetAreaEntityDetector()
        {
            if (_useCustomArea) return Args.Domain.GetSubDomain<CustomAreaSubDomain>().AreaEntityDetector;

            return Args.Owner.ComponentsContainer.Get<AreaEntityDetector>();
        }
    }
}