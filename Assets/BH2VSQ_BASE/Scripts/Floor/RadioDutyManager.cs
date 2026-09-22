using UdonSharp;

namespace BH2VSQ.Base
{
    public class RadioDutyManager : UdonSharpBehaviour
    {
        public AreaPopulationManager population;
        public TeleportManager teleport;
        public bool OnDuty()
        {
            if (population == null || teleport == null || teleport.points == null) return false;
            for (int i = 0; i < teleport.points.Length; i++)
                if (teleport.points[i] != null && teleport.points[i].radioDutyArea && population.Count(teleport.points[i].locationId) > 0) return true;
            return false;
        }
    }
}
