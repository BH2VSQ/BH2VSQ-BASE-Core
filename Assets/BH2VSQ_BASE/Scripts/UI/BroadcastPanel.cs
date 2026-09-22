using UdonSharp;
using TMPro;

namespace BH2VSQ.Base
{
    public class BroadcastPanel : UdonSharpBehaviour
    {
        public BroadcastManager manager;
        public TMP_InputField messageInput;
        public TMP_Text feedback;
        public LocalizationManager localization;
        public int priority;
        public void Send()
        {
            bool ok = manager != null && manager.SendBroadcast(messageInput.text, (BroadcastPriority)priority);
            if (feedback != null && localization != null) feedback.text = localization.Get(ok ? BaseText.Sent : BaseText.SendDenied);
            if (ok) messageInput.text = "";
        }
    }
}
