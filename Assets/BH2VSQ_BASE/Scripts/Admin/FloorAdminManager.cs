using UdonSharp;

namespace BH2VSQ.Base
{
    public class FloorAdminManager : UdonSharpBehaviour
    {
        public AdminManager admin;
        public FloorManager floors;
        public bool SetFloor(int floorId, int state)
        {
            if (admin == null || !admin.CanManage() || state < 0 || state > 2 || floors == null) return false;
            return floors.SetState(floorId, (FloorState)state, admin.permission);
        }
    }
}
