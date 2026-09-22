using UdonSharp;
using UnityEngine;

namespace BH2VSQ.Base
{
    public class TabMenuController : UdonSharpBehaviour
    {
        public PermissionManager permission;
        public GameObject personal;
        public GameObject teleport;
        public GameObject players;
        public GameObject admin;
        public GameObject adminTab;
        public void Refresh() { if (adminTab != null) adminTab.SetActive(permission != null && permission.IsAdmin()); }
        public void ShowPersonal() { Show(0); }
        public void ShowTeleport() { Show(1); }
        public void ShowPlayers() { Show(2); }
        public void ShowAdmin() { if (permission != null && permission.IsAdmin()) Show(3); }
        private void Show(int tab)
        {
            if (personal != null) personal.SetActive(tab == 0);
            if (teleport != null) teleport.SetActive(tab == 1);
            if (players != null) players.SetActive(tab == 2);
            if (admin != null) admin.SetActive(tab == 3 && permission != null && permission.IsAdmin());
        }
    }
}
