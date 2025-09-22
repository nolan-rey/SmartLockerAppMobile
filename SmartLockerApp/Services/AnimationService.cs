using CommunityToolkit.Maui.Extensions;

namespace SmartLockerApp.Services;

/// <summary>
/// Service d'animations simplifié utilisant CommunityToolkit.Maui
/// </summary>
public static class AnimationService
{
    // Durées par défaut pour une meilleure cohérence
    private const uint FastDuration = 200;
    private const uint NormalDuration = 300;
    private const uint SlowDuration = 400;

    /// <summary>
    /// Animation d'apparition en fondu
    /// </summary>
    public static async Task FadeInAsync(VisualElement element, uint duration = NormalDuration)
    {
        element.Opacity = 0;
        element.IsVisible = true;
        await element.FadeTo(1, duration, Easing.CubicOut);
    }

    /// <summary>
    /// Animation de disparition en fondu
    /// </summary>
    public static async Task FadeOutAsync(VisualElement element, uint duration = NormalDuration)
    {
        await element.FadeTo(0, duration, Easing.CubicIn);
        element.IsVisible = false;
    }

    /// <summary>
    /// Animation d'entrée depuis le bas avec fondu
    /// </summary>
    public static async Task SlideInFromBottomAsync(VisualElement element, uint duration = SlowDuration)
    {
        element.TranslationY = 50;
        element.Opacity = 0;
        element.IsVisible = true;
        
        await Task.WhenAll(
            element.TranslateTo(0, 0, duration, Easing.CubicOut),
            element.FadeTo(1, duration, Easing.CubicOut)
        );
    }

    /// <summary>
    /// Animation de sortie vers le bas avec fondu
    /// </summary>
    public static async Task SlideOutToBottomAsync(VisualElement element, uint duration = NormalDuration)
    {
        await Task.WhenAll(
            element.TranslateTo(0, 50, duration, Easing.CubicIn),
            element.FadeTo(0, duration, Easing.CubicIn)
        );
        element.IsVisible = false;
    }

    /// <summary>
    /// Animation d'apparition avec effet d'échelle
    /// </summary>
    public static async Task ScaleInAsync(VisualElement element, uint duration = NormalDuration)
    {
        element.Scale = 0.8;
        element.Opacity = 0;
        element.IsVisible = true;
        
        await Task.WhenAll(
            element.ScaleTo(1, duration, Easing.SpringOut),
            element.FadeTo(1, duration, Easing.CubicOut)
        );
    }

    /// <summary>
    /// Animation de pulsation pour attirer l'attention
    /// </summary>
    public static async Task PulseAsync(VisualElement element, uint duration = FastDuration)
    {
        var halfDuration = duration / 2;
        await element.ScaleTo(1.05, halfDuration, Easing.CubicOut);
        await element.ScaleTo(1, halfDuration, Easing.CubicIn);
    }

    /// <summary>
    /// Animation de secousse pour indiquer une erreur
    /// </summary>
    public static async Task ShakeAsync(VisualElement element)
    {
        const uint shakeDuration = 50;
        const double shakeDistance = 10;
        
        await element.TranslateTo(-shakeDistance, 0, shakeDuration);
        await element.TranslateTo(shakeDistance, 0, shakeDuration);
        await element.TranslateTo(-shakeDistance, 0, shakeDuration);
        await element.TranslateTo(shakeDistance, 0, shakeDuration);
        await element.TranslateTo(0, 0, shakeDuration);
    }

    /// <summary>
    /// Animation de pulsation continue pour les indicateurs de chargement
    /// </summary>
    public static async Task LoadingPulseAsync(VisualElement element, CancellationToken cancellationToken)
    {
        const uint pulseDuration = 800;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            await element.FadeTo(0.5, pulseDuration, Easing.SinInOut);
            if (cancellationToken.IsCancellationRequested) break;
            await element.FadeTo(1, pulseDuration, Easing.SinInOut);
        }
    }

    /// <summary>
    /// Animation de pression de bouton
    /// </summary>
    public static async Task ButtonPressAsync(VisualElement element)
    {
        const uint pressDuration = 100;
        await element.ScaleTo(0.95, pressDuration);
        await element.ScaleTo(1.0, pressDuration, Easing.CubicOut);
    }

    /// <summary>
    /// Animation de retournement de carte
    /// </summary>
    public static async Task CardFlipAsync(VisualElement element, uint duration = 600)
    {
        var halfDuration = duration / 2;
        await element.RotateYTo(90, halfDuration, Easing.CubicIn);
        await element.RotateYTo(0, halfDuration, Easing.CubicOut);
    }

    /// <summary>
    /// Animation de coche de succès
    /// </summary>
    public static async Task SuccessCheckmarkAsync(VisualElement element, uint duration = 800)
    {
        element.Scale = 0;
        element.Rotation = -45;
        element.IsVisible = true;
        
        // Animation d'apparition rapide
        await Task.WhenAll(
            element.FadeTo(0.8, 100),
            element.ScaleTo(0.95, 100)
        );
        
        // Animation finale avec rebond
        await element.ScaleTo(1, (uint)(duration * 0.4), Easing.CubicOut);
    }

    /// <summary>
    /// Animation combinée pour les transitions de page
    /// </summary>
    public static async Task PageTransitionAsync(VisualElement element, bool isEntering = true)
    {
        if (isEntering)
        {
            await SlideInFromBottomAsync(element);
        }
        else
        {
            await SlideOutToBottomAsync(element);
        }
    }

    /// <summary>
    /// Animation d'attention pour les éléments importants
    /// </summary>
    public static async Task AttentionAsync(VisualElement element)
    {
        await PulseAsync(element, 300);
        await Task.Delay(100);
        await PulseAsync(element, 300);
    }
}
