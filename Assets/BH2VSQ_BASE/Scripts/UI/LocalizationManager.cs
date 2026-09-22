using UdonSharp;
using TMPro;

namespace BH2VSQ.Base
{
    public class LocalizationManager : UdonSharpBehaviour
    {
        public PlayerDataManager data;
        public string[] english = { "Personal", "Teleport", "Players", "Admin", "Login", "Send", "Close", "Accept", "Reject", "Refresh", "Broadcast", "Floor" };
        public string[] chinese = { "个人", "传送", "玩家", "管理", "登录", "发送", "关闭", "接受", "拒绝", "刷新", "广播", "楼层" };

        public string Get(int key)
        {
            string[] table = data != null && data.language == 1 ? chinese : english;
            return key >= 0 && key < table.Length ? table[key] : "?";
        }
    }

}
