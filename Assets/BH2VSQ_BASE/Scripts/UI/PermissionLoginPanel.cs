using UdonSharp;
using UnityEngine;
using TMPro;

namespace BH2VSQ.Base
{
    public class PermissionLoginPanel : UdonSharpBehaviour
    {
        public TMP_InputField codeInput;
        public TOTPAuthManager auth;
        public TabMenuController menu;
        public RequestPanel notice;
        public LocalizationManager localization;

        public void Submit()
        {
            bool success = auth != null && codeInput != null && auth.Authenticate(codeInput.text);
            if (codeInput != null) codeInput.text = "";
            if (notice != null && localization != null) notice.ShowNotice(localization.Get(success ? BaseText.Authenticated : BaseText.InvalidCode));
            if (menu != null) menu.RefreshVisible();
        }
    }
}
