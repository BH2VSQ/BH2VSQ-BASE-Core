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
        public void Show(VRCPlayerApi requester, int type)
        {
            if (root != null) root.SetActive(true);
            if (message != null) message.text = Utilities.IsValid(requester) ? requester.displayName + (type == 0 ? " wants to teleport to you" : " invited you") : "Teleport request";
        }
        public void Hide() { if (root != null) root.SetActive(false); }
        public void SetSeconds(int seconds) { if (countdown != null) countdown.text = seconds + "s"; }
        public void Accept() { if (requests != null) requests.Accept(); Hide(); }
        public void Reject() { if (requests != null) requests.Reject(); Hide(); }
    }
}
