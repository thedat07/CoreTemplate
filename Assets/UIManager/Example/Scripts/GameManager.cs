using UnityEngine;
using UnityServiceLocator;

namespace BlitzyUI.UIExample
{
    public class GameManager : MonoBehaviour
    {
        public static readonly Screen.Id ScreenId_ExamplePopup = new Screen.Id("ExamplePopup");

        private IUIService uiService;

        private void Start()
        {
            uiService = ServiceLocator.Global.Get<IUIService>();

            uiService.OpenScreen(
                new Screen.Id("ExampleMenu"),
                null,
                "ExampleMenuScreen",
                () => Debug.Log("[GameManager] ExampleMenuScreen opened")
            );
        }
    }
}
