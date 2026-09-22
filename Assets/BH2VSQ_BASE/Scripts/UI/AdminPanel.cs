using UdonSharp;
using TMPro;
using UnityEngine;

namespace BH2VSQ.Base
{
    public class AdminPanel : UdonSharpBehaviour
    {
        public AdminManager admin;
        public FloorAdminManager floorAdmin;
        public FloorManager floors;
        public AreaPopulationManager population;
        public RadioDutyManager radio;
        public AreaManager areas;
        public TMP_Text status;
        public TMP_Text populationLeft;
        public TMP_Text populationRight;
        public GameObject populationRoot;
        public GameObject broadcastRoot;
        public int selectedFloor = 3;
        public int selectedState;

        public void Refresh()
        {
            if (status != null) status.text = admin != null && admin.CanManage() ?
                "Floor " + floors.floorNames[selectedFloor] + ": " + floors.GetState(selectedFloor) + "\n7F Radio: " + (radio.OnDuty() ? "On Duty" : "No Duty") : "Admin only";
            if (admin == null || !admin.CanManage() || areas == null || population == null) return;
            string left = "", right = "";
            for (int i = 0; i < areas.ids.Length; i++)
            {
                string line = areas.names[i] + ": " + population.Count(areas.ids[i]) + "\n";
                if (i < (areas.ids.Length + 1) / 2) left += line; else right += line;
            }
            if (populationLeft != null) populationLeft.text = left;
            if (populationRight != null) populationRight.text = right;
        }
        public void ShowPopulation()
        {
            if (admin == null || !admin.CanManage()) return;
            if (populationRoot != null) populationRoot.SetActive(true);
            if (broadcastRoot != null) broadcastRoot.SetActive(false);
            Refresh();
        }
        public void ShowBroadcast()
        {
            if (admin == null || !admin.CanManage()) return;
            if (populationRoot != null) populationRoot.SetActive(false);
            if (broadcastRoot != null) broadcastRoot.SetActive(true);
        }
        public bool CanOpenPlayerManagement() { return admin != null && admin.CanManage(); }
        public void ApplyFloorState()
        {
            if (floorAdmin != null) floorAdmin.SetFloor(selectedFloor, selectedState);
            Refresh();
        }
    }
}
