using UnityEngine;

namespace BH2VSQ.Base
{
    [CreateAssetMenu(menuName = "BH2VSQ BASE/Localization Database")]
    public class LocalizationDatabase : ScriptableObject
    {
        public string[] english = { "Personal", "Teleport", "Players", "Admin", "Login", "Send", "Close", "Accept", "Reject", "Refresh", "Broadcast", "Floor" };
        public string[] chinese = { "个人", "传送", "玩家", "管理", "登录", "发送", "关闭", "接受", "拒绝", "刷新", "广播", "楼层" };
    }
}
