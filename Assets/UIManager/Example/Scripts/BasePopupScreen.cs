using UnityServiceLocator;

public class BasePopupScreen : BlitzyUI.Screen
{
    protected IUIService uiService;

    public override void OnSetup()
    {
        uiService = ServiceLocator.Global.Get<IUIService>();
    }

    public override void OnPush(Data data)
    {
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

    protected virtual void CloseTopScreen()
    {
        uiService.CloseTopScreen();
    }
}
