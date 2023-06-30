namespace CompositeArchitecture
{
    public abstract class RootManager : CompositeManager
    {
        protected virtual bool DontInstallInstantiationHandler => false;
        
        public void Start ()
        {
            var container = new DependencyInjectionContainer();
            container.Bind(this, GetType());
            Install(container);
            if (DontInstallInstantiationHandler == false)
            {
                BindChild(new InstantiationHandler());
            }
            Inject();
            Initialize();
            Activate();
            
            Run();
        }

        protected virtual void Run() {}

        private void Update()
        {
            EarlyRefresh();
            Refresh();
            LateRefresh();
        }

        private void FixedUpdate()
        {
            FixedRefresh();
        }

        private void OnApplicationQuit()
        {
            if (State.IsDeinitialized == false)
            {
                if (State.IsActive)
                {
                    Deactivate();
                }

                Deinitialize();
            }
        }
    }
}