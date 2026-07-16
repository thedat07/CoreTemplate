using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePopupScreen : BlitzyUI.Screen
{
    private IUIService uiService;

    public override void OnSetup()
    {
        uiService = CoreBootstrapper.Services.UI;
    }

    public override void OnPush(Data data)
    {
        PushFinished();
    }

    public override void OnPop()
    {

    }

    public override void OnFocus()
    {
    }

    public override void OnFocusLost()
    {
    }

    public void CloseTopScreen()
    {
        uiService.CloseTopScreen();
    }
}
