using UdonSharp;
using TMPro;
using UnityEngine;

namespace BH2VSQ.Base
{
    public class TeleportPanel : UdonSharpBehaviour
    {
        public TeleportManager teleport;
        public PlayerDataManager data;
        public LocalizationManager localization;
        public TMP_Text feedback;
        public GameObject confirmRoot;
        public TMP_Text confirmText;
        public UIButtonAction[] locationActions;
        public TMP_Text[] locationLabels;
        public GameObject[] locationObjects;
        public GameObject previousButton;
        public GameObject nextButton;
        public TMP_Text pageLabel;
        private int page;
        private int pendingId;
        private bool hasPending;

        public void Refresh()
        {
            if (teleport == null || locationObjects == null) return;
            teleport.RefreshPoints();
            int visible = VisibleCount();
            int size = locationObjects.Length;
            if (size == 0) return;
            int pages = (visible + size - 1) / size;
            if (page >= pages) page = pages > 0 ? pages - 1 : 0;
            for (int i = 0; i < size; i++)
            {
                TeleportPoint point = VisibleAt(page * size + i);
                locationObjects[i].SetActive(point != null);
                if (point == null) continue;
                locationActions[i].value = point.locationId;
                locationLabels[i].text = point.DisplayName(localization.Language());
            }
            if (previousButton != null) previousButton.SetActive(page > 0);
            if (nextButton != null) nextButton.SetActive(page + 1 < pages);
            if (pageLabel != null) pageLabel.text = localization.Get(BaseText.Page) + " " + (page + 1) + " / " + (pages > 0 ? pages : 1);
        }

        private int VisibleCount()
        {
            int count = 0;
            if (teleport.points != null)
                for (int i = 0; i < teleport.points.Length; i++)
                    if (teleport.points[i] != null && teleport.points[i].tabVisible) count++;
            return count;
        }

        private TeleportPoint VisibleAt(int visibleIndex)
        {
            int count = 0;
            if (teleport.points != null)
                for (int i = 0; i < teleport.points.Length; i++)
                    if (teleport.points[i] != null && teleport.points[i].tabVisible)
                    {
                        if (count == visibleIndex) return teleport.points[i];
                        count++;
                    }
            return null;
        }

        public void NextPage() { page++; Refresh(); }
        public void PreviousPage() { if (page > 0) page--; Refresh(); }

        public void ToLocation(int id)
        {
            if (data != null && data.teleportConfirm) { Pending(id); return; }
            SetFeedback(teleport != null && teleport.ToLocation(id));
        }

        private void Pending(int id)
        {
            pendingId = id;
            hasPending = true;
            if (confirmRoot != null) confirmRoot.SetActive(true);
            TeleportPoint point = teleport == null ? null : teleport.ById(id);
            if (confirmText != null && localization != null)
                confirmText.text = localization.Get(BaseText.Teleport) + " " + (point == null ? id.ToString() : point.DisplayName(localization.Language())) + "?";
        }

        public void Confirm()
        {
            if (!hasPending) return;
            SetFeedback(teleport != null && teleport.ToLocation(pendingId));
            Cancel();
        }

        public void Cancel()
        {
            hasPending = false;
            if (confirmRoot != null) confirmRoot.SetActive(false);
        }

        private void SetFeedback(bool ok)
        {
            if (feedback != null && localization != null) feedback.text = localization.Get(ok ? BaseText.Teleporting : BaseText.AccessDenied);
        }
    }
}
