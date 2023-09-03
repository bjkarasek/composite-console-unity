using System;
using CompositeArchitecture;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CompositeConsole
{
    public class ConsoleManager : RootManager
    {
        public static ConsoleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ConsoleManager>();
                }
                
                return _instance;
            }
        }
        
        private static ConsoleManager _instance;
        
        public event Action OnConsoleOpened = delegate { };
        public event Action OnConsoleClosed = delegate { };

        [FormerlySerializedAs("settings")]
        [FormerlySerializedAs("Data")]
        [Header("Settings")]
        [SerializeField] public ConsoleSettings Settings;
        
        [Header("References")]
        [SerializeField] private Button ExitButton;
        [SerializeField] private ConsoleViewManager ConsoleViewManager;

        private CustomObjectRegistryController _customObjectRegistryController = new();
        
        private bool IsOpen => ConsoleViewManager.State.IsActive;

        protected override void OnInstall(DependencyInjectionContainer container)
        {
            BindChild(Settings);
            InstallChild(ConsoleViewManager, LifecycleFlags.WithoutActivate);
            InstallChild(new ShowLogsOnErrorController(), bindingMode: BindingMode.NonInjectable);
            InstallChild(_customObjectRegistryController);
        }

        protected override void OnInitialize()
        {
            ExitButton.onClick.AddListener(CloseConsole);
        }
        
        public void OpenConsole()
        {
            if (IsOpen == false)
            {
                ConsoleViewManager.Activate();
                OnConsoleOpened.Invoke();
            }
        }

        protected override void OnRefresh()
        {
            CheckShortcut();
        }

        private void CheckShortcut()
        {
            if (Input.GetKeyDown("`"))
            {
                if (IsOpen)
                {
                    CloseConsole();
                }
                else
                {
                    OpenConsole();
                }
            }
        }
        
        public void CloseConsole()
        {
            if (IsOpen)
            {
                ConsoleViewManager.Deactivate();
                OnConsoleClosed.Invoke();
            }            
        }

        public void RegisterDebugObject(IDebugBehaviour parent, object obj)
        {
            _customObjectRegistryController.RegisterDebugObject(parent, obj);
        }

        public void UnregisterDebugObject(IDebugBehaviour parent, object obj)
        {
            _customObjectRegistryController.UnregisterDebugObject(parent, obj);
        }

        protected override void OnDeinitialize()
        {
            ExitButton.onClick.RemoveListener(CloseConsole);
        }
    }
}