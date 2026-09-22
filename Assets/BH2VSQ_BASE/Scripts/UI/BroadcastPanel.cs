using UdonSharp;
using TMPro;

namespace BH2VSQ.Base
{
    public class BroadcastPanel : UdonSharpBehaviour
    {
        public BroadcastManager manager;
        public TMP_InputField messageInput;
        public TMP_Text feedback;
        public int priority;
        public void Send()
        {
            bool ok = manager != null && manager.SendBroadcast(messageInput.text, (BroadcastPriority)priority);
            if (feedback != null) feedback.text = ok ? "Sent" : "Admin only / empty message";
            if (ok) messageInput.text = "";
        }
    }
}
