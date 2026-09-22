using UdonSharp;
using VRC.SDK3.Persistence;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class PlayerDataManager : UdonSharpBehaviour
    {
        private const string XpKey = "bh2vsq.xp";
        private const string SecondsKey = "bh2vsq.seconds";
        public bool restored;
        public int experience;
        public int totalSeconds;

        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            experience = PlayerData.GetInt(player, XpKey);
            totalSeconds = PlayerData.GetInt(player, SecondsKey);
            restored = true;
        }

        public void Save()
        {
            if (!restored) return;
            PlayerData.SetInt(XpKey, experience);
            PlayerData.SetInt(SecondsKey, totalSeconds);
        }

        public int Level()
        {
            return 1 + (int)UnityEngine.Mathf.Sqrt(experience / 100f);
        }
    }
}
