using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityServiceLocator;

#pragma warning disable 0649

namespace BlitzyUI.UIExample
{
    public class ExamplePopupScreen : BasePopupScreen
    {
        public Text messageLabel;
        public Button okButton;

        public override void OnSetup()
        {
            base.OnSetup();
            okButton.onClick.AddListener(HandleOkClicked);
        }

        public override void OnPush(Data data)
        {
            base.OnPush(data);
            messageLabel.text = data.Get<string>("message");
        }

        private void HandleOkClicked()
        {
            CloseTopScreen();
        }
    }
}

#pragma warning restore 0649