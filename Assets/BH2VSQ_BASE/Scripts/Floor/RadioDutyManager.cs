using UdonSharp;

namespace BH2VSQ.Base
{
    public class RadioDutyManager : UdonSharpBehaviour
    {
        public AreaPopulationManager population;
        public bool OnDuty() { return population != null && population.Count(BaseConstants.RadioAreaId) > 0; }
    }
}
