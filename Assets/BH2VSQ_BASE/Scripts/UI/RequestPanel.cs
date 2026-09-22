using UdonSharp;
using UnityEngine;
using TMPro;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class RequestPanel : UdonSharpBehaviour
    {
        public GameObject root;
        public TMP_Text message;
        public TMP_Text countdown;
        public TeleportRequestManager requests;
        public LocalizationManager localization;
        public void Show(VRCPlayerApi requester, int type)
        {
            if (root != null) root.SetActive(true);
            if (message != null && localization != null)
                message.text = Utilities.IsValid(requester) ? requester.displayName + localization.Get(type == 0 ? BaseText.WantsToTeleport : BaseText.InvitedYou) : localization.Get(BaseText.TeleportRequest);
        }
        public void Hide() { if (root != null) root.SetActive(false); }
        public void SetSeconds(int seconds) { if (countdown != null && localization != null) countdown.text = seconds + localization.Get(BaseText.SecondsUnit); }
        public void Accept() { if (requests != null) requests.Accept(); Hide(); }
        public void Reject() { if (requests != null) requests.Reject(); Hide(); }
    }
}
