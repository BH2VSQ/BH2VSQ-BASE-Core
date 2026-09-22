using UdonSharp;

namespace BH2VSQ.Base
{
    public class UIButtonAction : UdonSharpBehaviour
    {
        public int action;
        public int value;
        public TabMenuController menu;
        public PermissionLoginPanel login;
        public TeleportPanel teleport;
        public PlayerListPanel players;
        public PlayerDetailPanel detail;
        public TeleportRequestManager requests;
        public RequestPanel requestPanel;
        public BroadcastNotificationManager notification;
        public AdminPanel admin;
        public BroadcastPanel broadcast;
        public PlayerDataManager data;

        public void Click()
        {
            if (action == 1 && menu != null) menu.ShowPersonal();
            else if (action == 2 && menu != null) { menu.ShowTeleport(); if (teleport != null) teleport.Refresh(); }
            else if (action == 3 && menu != null) { menu.ShowPlayers(); if (players != null) players.Refresh(); }
            else if (action == 4 && menu != null) { menu.ShowAdmin(); if (admin != null) admin.Refresh(); }
            else if (action == 10 && login != null) login.Submit();
            else if (action == 20 && teleport != null) teleport.ToLocation(value);
            else if (action == 22 && teleport != null) teleport.Confirm();
            else if (action == 23 && teleport != null) teleport.Cancel();
            else if (action == 26 && teleport != null) teleport.PreviousPage();
            else if (action == 27 && teleport != null) teleport.NextPage();
            else if (action == 30 && players != null) players.Refresh();
            else if (action == 31 && detail != null) detail.ShowPlayer(value);
            else if (action == 32 && detail != null && requests != null) requests.Send(detail.selectedPlayerId, value);
            else if (action == 40 && requestPanel != null) requestPanel.Accept();
            else if (action == 41 && requestPanel != null) requestPanel.Reject();
            else if (action == 42 && notification != null) notification.Close();
            else if (action == 50 && broadcast != null) broadcast.Send();
            else if (action == 51 && broadcast != null) broadcast.priority = value;
            else if (action == 60 && admin != null) { admin.selectedFloor = value; admin.Refresh(); }
            else if (action == 61 && admin != null) { admin.selectedState = value; admin.Refresh(); }
            else if (action == 62 && admin != null) admin.ApplyFloorState();
            else if (action == 63 && admin != null) admin.ShowPopulation();
            else if (action == 64 && admin != null) admin.ShowBroadcast();
            else if (action == 65 && admin != null && admin.CanOpenPlayerManagement() && menu != null)
            {
                menu.ShowPlayers();
                if (players != null) players.Refresh();
            }
            else if (action == 66 && admin != null) admin.PreviousFloorPage();
            else if (action == 67 && admin != null) admin.NextFloorPage();
            else if (action == 68 && admin != null) admin.PreviousPopulationPage();
            else if (action == 69 && admin != null) admin.NextPopulationPage();
            else if (action == 70 && data != null) { data.language = value; data.Save(); }
            else if (action == 71 && data != null) { data.teleportConfirm = !data.teleportConfirm; data.Save(); }
            else if (action == 72 && data != null) { data.notificationsEnabled = !data.notificationsEnabled; data.Save(); }
        }
    }
}
