using System;
using CompositeArchitecture;
using UnityEngine;
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

        [SerializeField] private Button ExitButton;
        [SerializeField] private ConsoleViewManager ConsoleViewManager;
        
        private bool IsOpen => ConsoleViewManager.State.IsActive;

        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(ConsoleViewManager, LifecycleFlags.WithoutActivate);
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

        protected override void OnDeinitialize()
        {
            ExitButton.onClick.RemoveListener(CloseConsole);
        }
    }
}