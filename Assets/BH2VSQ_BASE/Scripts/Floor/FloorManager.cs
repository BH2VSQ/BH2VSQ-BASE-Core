using UdonSharp;

namespace BH2VSQ.Base
{
    // Floor IDs and names come from TeleportPoint instances. State is synced on each point.
    public class FloorManager : UdonSharpBehaviour
    {
        public TeleportManager teleport;

        public bool Valid(int floorId) { return teleport != null && teleport.ByFloor(floorId) != null; }

        public string GetFloor(int floorId, int language)
        {
            TeleportPoint point = teleport == null ? null : teleport.ByFloor(floorId);
            return point == null ? "?" : point.DisplayFloor(language);
        }

        public FloorState GetState(int floorId)
        {
            TeleportPoint point = teleport == null ? null : teleport.ByFloor(floorId);
            return point == null ? FloorState.Maintenance : (FloorState)point.floorState;
        }

        public bool IsFloorAvailable(int floorId) { return Valid(floorId) && GetState(floorId) == FloorState.Open; }

        public bool SetState(int floorId, FloorState state, PermissionManager permission)
        {
            if (!IsGuestFloor(floorId) || permission == null || !permission.IsAdmin() || teleport.points == null) return false;
            for (int i = 0; i < teleport.points.Length; i++)
                if (teleport.points[i] != null && teleport.points[i].floorId == floorId)
                    teleport.points[i].SetFloorState(state);
            return true;
        }

        public bool IsGuestFloor(int floorId)
        {
            if (teleport == null || teleport.points == null) return false;
            bool found = false;
            for (int i = 0; i < teleport.points.Length; i++)
            {
                TeleportPoint point = teleport.points[i];
                if (point == null || point.floorId != floorId) continue;
                found = true;
                if (point.requiredRank != BaseRank.Visitor) return false;
            }
            return found;
        }

        public int ManageableFloorCount()
        {
            if (teleport == null || teleport.points == null) return 0;
            int count = 0;
            for (int i = 0; i < teleport.points.Length; i++)
            {
                TeleportPoint point = teleport.points[i];
                if (point == null || !IsGuestFloor(point.floorId)) continue;
                bool seen = false;
                for (int j = 0; j < i; j++)
                    if (teleport.points[j] != null && teleport.points[j].floorId == point.floorId) { seen = true; break; }
                if (!seen) count++;
            }
            return count;
        }

        public int ManageableFloorIdAt(int index)
        {
            if (teleport == null || teleport.points == null) return BaseConstants.InvalidId;
            int position = 0;
            for (int i = 0; i < teleport.points.Length; i++)
            {
                TeleportPoint point = teleport.points[i];
                if (point == null || !IsGuestFloor(point.floorId)) continue;
                bool seen = false;
                for (int j = 0; j < i; j++)
                    if (teleport.points[j] != null && teleport.points[j].floorId == point.floorId) { seen = true; break; }
                if (seen) continue;
                if (position == index) return point.floorId;
                position++;
            }
            return BaseConstants.InvalidId;
        }

        public int UniqueFloorCount()
        {
            if (teleport == null || teleport.points == null) return 0;
            int count = 0;
            for (int i = 0; i < teleport.points.Length; i++)
            {
                if (teleport.points[i] == null) continue;
                bool seen = false;
                for (int j = 0; j < i; j++)
                    if (teleport.points[j] != null && teleport.points[j].floorId == teleport.points[i].floorId) { seen = true; break; }
                if (!seen) count++;
            }
            return count;
        }

        public int FloorIdAt(int index)
        {
            if (teleport == null || teleport.points == null) return BaseConstants.InvalidId;
            int position = 0;
            for (int i = 0; i < teleport.points.Length; i++)
            {
                if (teleport.points[i] == null) continue;
                bool seen = false;
                for (int j = 0; j < i; j++)
                    if (teleport.points[j] != null && teleport.points[j].floorId == teleport.points[i].floorId) { seen = true; break; }
                if (seen) continue;
                if (position == index) return teleport.points[i].floorId;
                position++;
            }
            return BaseConstants.InvalidId;
        }
    }
}
