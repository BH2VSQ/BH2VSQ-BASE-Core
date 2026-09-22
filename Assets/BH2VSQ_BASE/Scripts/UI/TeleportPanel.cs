using UdonSharp;
using TMPro;
using UnityEngine;

namespace BH2VSQ.Base
{
    public class TeleportPanel : UdonSharpBehaviour
    {
        public TeleportManager teleport;
        public PlayerDataManager data;
        public TMP_Text feedback;
        public GameObject confirmRoot;
        public TMP_Text confirmText;
        private int pendingId = -1;
        private bool pendingArea;
        public void ToFloor(int id)
        {
            if (data != null && data.teleportConfirm) { Pending(id, false); return; }
            bool ok = teleport != null && teleport.ToFloor(id);
            if (feedback != null) feedback.text = ok ? "Teleporting" : "Access denied or point missing";
        }
        public void ToArea(int id)
        {
            if (data != null && data.teleportConfirm) { Pending(id, true); return; }
            bool ok = teleport != null && teleport.ToArea(id);
            if (feedback != null) feedback.text = ok ? "Teleporting" : "Access denied or point missing";
        }
        private void Pending(int id, bool area)
        {
            pendingId = id; pendingArea = area;
            if (confirmRoot != null) confirmRoot.SetActive(true);
            if (confirmText != null) confirmText.text = "Teleport to " + (area ? "area " : "floor ") + id + "?";
        }
        public void Confirm()
        {
            if (pendingId < 0) return;
            bool ok = teleport != null && (pendingArea ? teleport.ToArea(pendingId) : teleport.ToFloor(pendingId));
            if (feedback != null) feedback.text = ok ? "Teleporting" : "Access denied or point missing";
            Cancel();
        }
        public void Cancel()
        {
            pendingId = -1;
            if (confirmRoot != null) confirmRoot.SetActive(false);
        }
    }
}
