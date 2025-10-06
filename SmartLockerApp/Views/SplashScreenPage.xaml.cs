using SmartLockerApp.ViewModels;

namespace SmartLockerApp.Views;

public partial class SplashScreenPage : ContentPage
{
    private bool _animationsCompleted = false;

    public SplashScreenPage(SplashScreenPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Démarrer les animations
        await StartAnimations();
        
        if (BindingContext is SplashScreenPageViewModel viewModel)
        {
            await viewModel.StartSplashSequenceCommand.ExecuteAsync(null);
        }
    }

    private async Task StartAnimations()
    {
        var logoAnimation = AnimateLogo();
        var textAnimation = AnimateText();
        _ = AnimateLoadingDots();
        
        await Task.WhenAll(logoAnimation, textAnimation);
    }

    private async Task StartSplashSequence()
    {
        try
        {
            // Démarrer les animations en parallèle
            var logoAnimation = AnimateLogo();
            var textAnimation = AnimateText();
            var loadingAnimation = AnimateLoadingDots();
            
            // Attendre que toutes les animations se terminent
            await Task.WhenAll(logoAnimation, textAnimation);
            
            // Simuler le chargement de l'application
            await SimulateAppLoading();
            
            _animationsCompleted = true;
            
            // Naviguer vers la page appropriée
            await NavigateToMainApp();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur SplashScreen: {ex.Message}");
            // En cas d'erreur, naviguer quand même
            await NavigateToMainApp();
        }
    }

    private async Task AnimateLogo()
    {
        // Animation d'entrée du logo
        LogoImage.Scale = 0.3;
        LogoImage.Opacity = 0;
        
        // Animation de scale et fade in
        var scaleAnimation = LogoImage.ScaleTo(1.0, 800, Easing.BounceOut);
        var fadeAnimation = LogoImage.FadeTo(1.0, 600, Easing.CubicOut);
        
        await Task.WhenAll(scaleAnimation, fadeAnimation);
        
        // Animation de pulsation subtile
        _ = Task.Run(async () =>
        {
            while (!_animationsCompleted)
            {
                await LogoImage.ScaleTo(1.05, 1000, Easing.SinInOut);
                await LogoImage.ScaleTo(1.0, 1000, Easing.SinInOut);
            }
        });
    }

    private async Task AnimateText()
    {
        // Attendre un peu avant d'animer le texte
        await Task.Delay(400);
        
        // Animation du nom de l'app
        AppNameLabel.TranslationY = 30;
        AppNameLabel.Opacity = 0;
        
        var slideAnimation = AppNameLabel.TranslateTo(0, 0, 600, Easing.CubicOut);
        var fadeAnimation = AppNameLabel.FadeTo(1.0, 600, Easing.CubicOut);
        
        await Task.WhenAll(slideAnimation, fadeAnimation);
        
        // Animation du tagline
        await Task.Delay(200);
        TaglineLabel.TranslationY = 20;
        TaglineLabel.Opacity = 0;
        
        var taglineSlide = TaglineLabel.TranslateTo(0, 0, 500, Easing.CubicOut);
        var taglineFade = TaglineLabel.FadeTo(1.0, 500, Easing.CubicOut);
        
        await Task.WhenAll(taglineSlide, taglineFade);
    }

    private async Task AnimateLoadingDots()
    {
        await Task.Delay(800);
        
        // Animation continue des points de chargement
        _ = Task.Run(async () =>
        {
            while (!_animationsCompleted)
            {
                // Animation des points en séquence
                await LoadingDot1.FadeTo(1.0, 300);
                await Task.Delay(100);
                await LoadingDot2.FadeTo(1.0, 300);
                await Task.Delay(100);
                await LoadingDot3.FadeTo(1.0, 300);
                await Task.Delay(300);
                
                // Fade out
                await LoadingDot1.FadeTo(0.3, 300);
                await LoadingDot2.FadeTo(0.6, 300);
                await LoadingDot3.FadeTo(0.9, 300);
                await Task.Delay(200);
            }
        });
    }

    private async Task SimulateAppLoading()
    {
        // Simuler le chargement des services
        var loadingTasks = new[]
        {
            "Initialisation des services...",
            "Chargement des données...",
            "Vérification de la connexion...",
            "Finalisation..."
        };

        foreach (var task in loadingTasks)
        {
            LoadingLabel.Text = task;
            await Task.Delay(500); // Simule le temps de chargement
        }
        
        LoadingLabel.Text = "Prêt !";
        await Task.Delay(300);
    }

    private async Task NavigateToMainApp()
    {
        try
        {
            // Animation de sortie
            var fadeOut = this.FadeTo(0, 300, Easing.CubicIn);
            await fadeOut;
            
            // Toujours naviguer vers la page de connexion après le chargement
            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur navigation: {ex.Message}");
            // Fallback vers la page de connexion
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
