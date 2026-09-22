using UdonSharp;
using UnityEngine;
using TMPro;

namespace BH2VSQ.Base
{
    public class PermissionLoginPanel : UdonSharpBehaviour
    {
        public TMP_InputField codeInput;
        public TMP_Text feedback;
        public GameObject loginRoot;
        public TOTPAuthManager auth;
        public TabMenuController menu;
        public LocalizationManager localization;

        public void Submit()
        {
            bool success = auth != null && auth.Authenticate(codeInput.text);
            if (feedback != null && localization != null) feedback.text = localization.Get(success ? BaseText.Authenticated : BaseText.InvalidCode);
            if (success && loginRoot != null) loginRoot.SetActive(false);
            if (menu != null) menu.Refresh();
        }
    }
}
