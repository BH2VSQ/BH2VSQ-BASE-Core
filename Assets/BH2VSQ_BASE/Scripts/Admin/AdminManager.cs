using UdonSharp;

namespace BH2VSQ.Base
{
    public class AdminManager : UdonSharpBehaviour
    {
        public PermissionManager permission;
        public bool CanManage() { return permission != null && permission.IsAdmin(); }
    }
}
