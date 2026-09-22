using UdonSharp;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class AccessManager : UdonSharpBehaviour
    {
        public FloorManager floors;
        public AreaManager areas;
        public PermissionManager permission;

        public AccessResult CheckFloorAccess(int floorId)
        {
            if (!Utilities.IsValid(Networking.LocalPlayer)) return AccessResult.InvalidPlayer;
            if (floors == null || !floors.Valid(floorId)) return AccessResult.InvalidFloor;
            if (permission == null || (int)permission.GetRank() < floors.requiredRanks[floorId]) return AccessResult.InsufficientRank;
            if (permission.IsAdmin()) return AccessResult.Allowed;
            FloorState state = floors.GetState(floorId);
            if (state == FloorState.Reserved) return AccessResult.FloorReserved;
            if (state == FloorState.Maintenance) return AccessResult.FloorMaintenance;
            return AccessResult.Allowed;
        }

        public AccessResult CheckAreaAccess(int areaId)
        {
            if (areas == null) return AccessResult.InvalidArea;
            int index = areas.IndexOf(areaId);
            if (index < 0) return AccessResult.InvalidArea;
            AccessResult floor = CheckFloorAccess(areas.floorIds[index]);
            if (floor != AccessResult.Allowed) return floor;
            return (int)permission.GetRank() >= areas.requiredRanks[index] ? AccessResult.Allowed : AccessResult.InsufficientRank;
        }
    }
}
