using UnityEngine;
using UnityEngine.SceneManagement;
using UnityServiceLocator;

public class CoreBootstrapper : Bootstrapper
{
    [Header("Prefabs")]
    [SerializeField] private AudioService audioServicePrefab;

    /// <summary>Static cache — survives scene loads when ServiceLocator.Global resets.</summary>
    public static class Services
    {
        public static IDataService Data { get; internal set; }
        public static IGameStateService GameState { get; internal set; }
        public static IVibrationService Vibration { get; internal set; }
        public static IUIService UI { get; internal set; }
        public static ISceneService Scene { get; internal set; }
        public static IAudioService Audio { get; internal set; }
    }

    protected override void Bootstrap()
    {
        // --- GLOBAL SERVICES (tồn tại suốt game) ---

        // Data
        if (!Container.TryGet(out IDataService _))
        {
            var svc = new DataService();
            Container.Register<IDataService>(svc);
            Services.Data = svc;
            Debug.Log("[CoreBootstrapper] DataService registered");
        }

        // GameState
        if (!Container.TryGet(out IGameStateService _))
        {
            var svc = new GameStateService();
            svc.ChangeState(GameState.Boot);
            Container.Register<IGameStateService>(svc);
            Services.GameState = svc;
            Debug.Log("[CoreBootstrapper] GameStateService registered");
        }

        // Vibration
        if (!Container.TryGet(out IVibrationService _))
        {
            var svc = new VibrationService(Services.Data);
            Container.Register<IVibrationService>(svc);
            Services.Vibration = svc;
            Debug.Log("[CoreBootstrapper] VibrationService registered");
        }

        // UI
        if (!Container.TryGet(out IUIService _))
        {
            var svc = new UIService();
            Container.Register<IUIService>(svc);
            Services.UI = svc;
            Debug.Log("[CoreBootstrapper] UIService registered");
        }

        // Scene
        if (!Container.TryGet(out ISceneService _))
        {
            var svc = new SceneService();
            Container.Register<ISceneService>(svc);
            Services.Scene = svc;
            Debug.Log("[CoreBootstrapper] SceneService registered");
        }

        // Audio
        if (!Container.TryGet(out IAudioService _))
        {
            var audio = Instantiate(audioServicePrefab);
            audio.Initialize(Services.Data);
            Container.Register<IAudioService>(audio);
            Services.Audio = audio;
            Debug.Log("[CoreBootstrapper] AudioService registered");
        }

    }

    private void Start()
    {
        // Sau khi tất cả services đã init xong → load scene UIExample
        Services.Scene.LoadSceneAsync("UIExample", LoadSceneMode.Single,
            onLoaded: () =>
            {
                Services.GameState.ChangeState(GameState.MainMenu);
                Debug.Log("[CoreBootstrapper] Scene UIExample loaded → MainMenu");
            }
        );
    }
}