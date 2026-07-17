using UnityEngine;
using UnityEngine.SceneManagement;
using UnityServiceLocator;

public class CoreBootstrapper : MonoBehaviour
{
    /// <summary>Static cache cho các scene scripts cần truy cập nhanh.</summary>
    public static class Services
    {
        public static ILogEventService Trigger { get; set; }
    }

    ServiceLocator Container => ServiceLocator.Global;

    private void Start()
    {
        Bootstrap();

        // Sau khi tất cả services đã init xong → load scene UIExample
        var sceneService = Container.Get<ISceneService>();
        var gameState = Container.Get<IGameStateService>();

        sceneService.LoadSceneAsync("UIExample", LoadSceneMode.Single,
            onLoaded: () =>
            {
                gameState.ChangeState(GameState.MainMenu);
                Debug.Log("[CoreBootstrapper] Scene UIExample loaded → MainMenu");
            }
        );
    }

    void Bootstrap()
    {
        // --- GLOBAL SERVICES (tồn tại suốt game) ---

        // Data
        if (!Container.TryGet(out IDataService _))
        {
            Container.Register<IDataService>(new DataService());
            Debug.Log("[CoreBootstrapper] DataService registered");
        }

        // GameState
        if (!Container.TryGet(out IGameStateService _))
        {
            var gss = new GameStateService();
            gss.ChangeState(GameState.Boot);
            Container.Register<IGameStateService>(gss);
            Debug.Log("[CoreBootstrapper] GameStateService registered");
        }

        // Vibration
        if (!Container.TryGet(out IVibrationService _))
        {
            var dataService = Container.Get<IDataService>();
            Container.Register<IVibrationService>(new VibrationService(dataService));
            Debug.Log("[CoreBootstrapper] VibrationService registered");
        }

        // UI
        if (!Container.TryGet(out IUIService _))
        {
            Container.Register<IUIService>(new UIService());
            Debug.Log("[CoreBootstrapper] UIService registered");
        }

        // Scene
        if (!Container.TryGet(out ISceneService _))
        {
            Container.Register<ISceneService>(new SceneService());
            Debug.Log("[CoreBootstrapper] SceneService registered");
        }

        // Trigger
        if (!Container.TryGet(out ILogEventService _))
        {
            var svc = new LogEventService();
            Container.Register<ILogEventService>(svc);
            Services.Trigger = svc;
            Debug.Log("[CoreBootstrapper] TriggerService registered");
        }

        // Audio
        if (!Container.TryGet(out IAudioService _))
        {
            var dataService = Container.Get<IDataService>();
            var audio = new AudioService();
            audio.Initialize(dataService);
            Container.Register<IAudioService>(audio);
            Debug.Log("[CoreBootstrapper] AudioService registered");
        }

        // PowerUp
        if (!Container.TryGet(out IPowerUpService _))
        {
            Container.Register<IPowerUpService>(new PowerUpService());
            Debug.Log("[CoreBootstrapper] PowerUpService registered");
        }
    }
}