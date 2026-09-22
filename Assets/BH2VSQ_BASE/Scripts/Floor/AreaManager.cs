using UdonSharp;

namespace BH2VSQ.Base
{
    // Compatibility facade for location and population views; TeleportPoint is the data source.
    public class AreaManager : UdonSharpBehaviour
    {
        public TeleportManager teleport;

        public int Count() { return teleport == null || teleport.points == null ? 0 : teleport.points.Length; }

        public int IndexOf(int locationId)
        {
            if (teleport == null || teleport.points == null) return -1;
            for (int i = 0; i < teleport.points.Length; i++)
                if (teleport.points[i] != null && teleport.points[i].locationId == locationId) return i;
            return -1;
        }

        public int IdAt(int index)
        {
            TeleportPoint point = PointAt(index);
            return point == null ? BaseConstants.InvalidId : point.locationId;
        }

        public int FloorIdAt(int index)
        {
            TeleportPoint point = PointAt(index);
            return point == null ? BaseConstants.InvalidId : point.floorId;
        }

        public float XpMultiplierAt(int index)
        {
            TeleportPoint point = PointAt(index);
            return point == null ? 1f : point.xpMultiplier;
        }

        public bool IsRadioLocation(int locationId)
        {
            TeleportPoint point = teleport == null ? null : teleport.ById(locationId);
            return point != null && point.radioDutyArea;
        }

        public string DisplayName(int index, int language)
        {
            TeleportPoint point = PointAt(index);
            return point == null ? "?" : point.DisplayName(language);
        }

        private TeleportPoint PointAt(int index)
        {
            if (teleport == null || teleport.points == null || index < 0 || index >= teleport.points.Length) return null;
            return teleport.points[index];
        }
    }
}
