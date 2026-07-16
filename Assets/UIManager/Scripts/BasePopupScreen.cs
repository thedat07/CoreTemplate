using BlitzyUI;
using UnityServiceLocator;

public class BasePopupScreen : EmptyScreen
{
    protected IUIService uiService;

    public override void OnSetup()
    {
        base.OnSetup();
        uiService = ServiceLocator.Global.Get<IUIService>();
    }

    protected virtual void CloseTopScreen()
    {
        uiService.CloseTopScreen();
    }
}
