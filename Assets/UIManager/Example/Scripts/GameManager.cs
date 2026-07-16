using UnityEngine;

namespace BlitzyUI.UIExample
{
    public class GameManager : MonoBehaviour
    {
        public static readonly Screen.Id ScreenId_ExamplePopup = new Screen.Id("ExamplePopup");

        private IUIService uiService;

        private void Start()
        {
            uiService = CoreBootstrapper.Services.UI;

            uiService.ShowLoading("Preparing...");

            uiService.OpenScreen(
                new Screen.Id("ExampleMenu"),
                null,
                "ExampleMenuScreen",
                () =>
                {
                    // Load xong menu rồi mới tắt loading
                    uiService.HideLoading();
                }
            );
        }
    }
}
