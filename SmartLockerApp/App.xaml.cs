namespace SmartLockerApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var shell = new AppShell();
            
            // Démarrer avec l'écran de chargement
            shell.GoToAsync("//SplashScreenPage");
            
            return new Window(shell);
        }
    }
}