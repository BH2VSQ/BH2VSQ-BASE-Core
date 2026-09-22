using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class BaseWorldSystem : UdonSharpBehaviour
    {
        public PlayerRegistry registry;
        public PlayerDataManager playerData;
        public PlayerAreaTracker tracker;
        public AuthenticationSession session;
        public FloorManager floors;
        public AccessManager access;
        public TeleportManager teleport;
        public TabMenuController menu;
        public GameObject menuCanvas;
        public LocalCanvasFollower menuFollower;
        public CanvasGroup menuCanvasGroup;
        [Range(0f, 1f)] public float menuOpacity = .86f;
        public bool ready;

        private void Start()
        {
            if (menuCanvas != null) menuCanvas.SetActive(false);
            if (menuCanvasGroup != null) menuCanvasGroup.alpha = Mathf.Clamp01(menuOpacity);
            ready = registry != null && playerData != null && tracker != null && session != null && floors != null && access != null && teleport != null;
            if (!ready) Debug.LogError("BH2VSQ BASE：核心引用缺失，请运行配置验证。");
            else
            {
                registry.Refresh();
                session.ResetSession();
                if (menu != null) menu.Refresh();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab)) ToggleMenu();
        }

        public void ToggleMenu()
        {
            if (menuCanvas == null) return;
            bool show = !menuCanvas.activeSelf;
            menuCanvas.SetActive(show);
            if (!show) return;
            if (menuFollower != null) menuFollower.Place();
            if (menu != null) menu.Refresh();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (registry != null) registry.Refresh();
            if (menu != null) menu.Refresh();
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (registry != null) registry.Refresh();
            if (menu != null) menu.Refresh();
        }
    }
}
