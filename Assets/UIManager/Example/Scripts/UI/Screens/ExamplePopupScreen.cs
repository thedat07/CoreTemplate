using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 0649

namespace BlitzyUI.UIExample
{
    public class ExamplePopupScreen : BlitzyUI.Screen
    {
        public Text messageLabel;
        public Button okButton;

        private IUIService uiService;

        public override void OnSetup()
        {
            uiService = CoreBootstrapper.Services.UI;
            okButton.onClick.AddListener(HandleOkClicked);
        }

        public override void OnPush(Data data)
        {
            messageLabel.text = data.Get<string>("message");
            PushFinished();
        }

        public override void OnPop()
        {
            PopFinished();
        }

        public override void OnFocus()
        {
        }

        public override void OnFocusLost()
        {
        }

        private void HandleOkClicked ()
        {
            uiService.CloseTopScreen();
        }
    }
}

#pragma warning restore 0649