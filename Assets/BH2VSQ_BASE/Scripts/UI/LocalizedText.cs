using UdonSharp;
using TMPro;

namespace BH2VSQ.Base
{
    public class LocalizedText : UdonSharpBehaviour
    {
        public LocalizationManager localization;
        public TMP_Text target;
        public int key;
        private int lastLanguage = -1;
        private void Start() { SendCustomEventDelayedSeconds("Refresh", 1f); }
        public void Refresh()
        {
            if (localization != null && target != null && localization.data != null && lastLanguage != localization.data.language)
            {
                lastLanguage = localization.data.language;
                target.text = localization.Get(key);
            }
            SendCustomEventDelayedSeconds("Refresh", 2f);
        }
    }
}
