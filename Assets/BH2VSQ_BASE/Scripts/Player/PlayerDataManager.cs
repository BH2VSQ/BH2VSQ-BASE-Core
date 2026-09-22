using UdonSharp;
using VRC.SDK3.Persistence;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class PlayerDataManager : UdonSharpBehaviour
    {
        private const string XpKey = "bh2vsq.xp";
        private const string SecondsKey = "bh2vsq.seconds";
        private const string LanguageKey = "bh2vsq.language";
        private const string ConfirmKey = "bh2vsq.teleportConfirm";
        private const string NotificationsKey = "bh2vsq.notifications";
        private const string VolumeKey = "bh2vsq.notificationVolume";
        public bool restored;
        public int experience;
        public int totalSeconds;
        public int language;
        public bool teleportConfirm = true;
        public bool notificationsEnabled = true;
        public float notificationVolume = .6f;

        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            experience = PlayerData.GetInt(player, XpKey);
            totalSeconds = PlayerData.GetInt(player, SecondsKey);
            language = PlayerData.GetInt(player, LanguageKey);
            bool setting;
            if (PlayerData.TryGetBool(player, ConfirmKey, out setting)) teleportConfirm = setting;
            if (PlayerData.TryGetBool(player, NotificationsKey, out setting)) notificationsEnabled = setting;
            float volume;
            if (PlayerData.TryGetFloat(player, VolumeKey, out volume)) notificationVolume = UnityEngine.Mathf.Clamp01(volume);
            restored = true;
        }

        public void Save()
        {
            if (!restored) return;
            PlayerData.SetInt(XpKey, experience);
            PlayerData.SetInt(SecondsKey, totalSeconds);
            PlayerData.SetInt(LanguageKey, language);
            PlayerData.SetBool(ConfirmKey, teleportConfirm);
            PlayerData.SetBool(NotificationsKey, notificationsEnabled);
            PlayerData.SetFloat(VolumeKey, notificationVolume);
        }

        public int Level()
        {
            return 1 + (int)UnityEngine.Mathf.Sqrt(experience / 100f);
        }
    }
}
