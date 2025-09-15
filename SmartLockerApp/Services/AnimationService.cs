namespace SmartLockerApp.Services;

public static class AnimationService
{
    public static async Task FadeInAsync(VisualElement element, uint duration = 300)
    {
        element.Opacity = 0;
        element.IsVisible = true;
        await element.FadeTo(1, duration, Easing.CubicOut);
    }

    public static async Task FadeOutAsync(VisualElement element, uint duration = 300)
    {
        await element.FadeTo(0, duration, Easing.CubicIn);
        element.IsVisible = false;
    }

    public static async Task SlideInFromBottomAsync(VisualElement element, uint duration = 400)
    {
        element.TranslationY = 50;
        element.Opacity = 0;
        element.IsVisible = true;
        
        var tasks = new[]
        {
            element.TranslateTo(0, 0, duration, Easing.CubicOut),
            element.FadeTo(1, duration, Easing.CubicOut)
        };
        
        await Task.WhenAll(tasks);
    }

    public static async Task SlideOutToBottomAsync(VisualElement element, uint duration = 300)
    {
        var tasks = new[]
        {
            element.TranslateTo(0, 50, duration, Easing.CubicIn),
            element.FadeTo(0, duration, Easing.CubicIn)
        };
        
        await Task.WhenAll(tasks);
        element.IsVisible = false;
    }

    public static async Task ScaleInAsync(VisualElement element, uint duration = 300)
    {
        element.Scale = 0.8;
        element.Opacity = 0;
        element.IsVisible = true;
        
        var tasks = new[]
        {
            element.ScaleTo(1, duration, Easing.SpringOut),
            element.FadeTo(1, duration, Easing.CubicOut)
        };
        
        await Task.WhenAll(tasks);
    }

    public static async Task PulseAsync(VisualElement element, uint duration = 200)
    {
        await element.ScaleTo(1.05, duration / 2, Easing.CubicOut);
        await element.ScaleTo(1, duration / 2, Easing.CubicIn);
    }

    public static async Task ShakeAsync(VisualElement element, uint duration = 500)
    {
        var tasks = new[]
        {
            element.TranslateTo(-10, 0, 50),
            Task.Delay(50).ContinueWith(_ => element.TranslateTo(10, 0, 50)),
            Task.Delay(100).ContinueWith(_ => element.TranslateTo(-10, 0, 50)),
            Task.Delay(150).ContinueWith(_ => element.TranslateTo(10, 0, 50)),
            Task.Delay(200).ContinueWith(_ => element.TranslateTo(0, 0, 50))
        };
        
        await Task.WhenAll(tasks);
    }

    public static async Task LoadingPulseAsync(VisualElement element, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await element.FadeTo(0.5, 800, Easing.SinInOut);
            if (cancellationToken.IsCancellationRequested) break;
            await element.FadeTo(1, 800, Easing.SinInOut);
        }
    }

    public static async Task ButtonPressAsync(VisualElement element)
    {
        await element.ScaleTo(0.95, 100, Easing.CubicOut);
        await element.ScaleTo(1, 100, Easing.CubicOut);
    }

    public static async Task CardFlipAsync(VisualElement element, uint duration = 600)
    {
        await element.RotateYTo(90, duration / 2, Easing.CubicIn);
        await element.RotateYTo(0, duration / 2, Easing.CubicOut);
    }

    public static async Task SuccessCheckmarkAsync(VisualElement element, uint duration = 800)
    {
        element.Scale = 0;
        element.Rotation = -45;
        element.IsVisible = true;
        
        var tasks = new[]
        {
            element.ScaleTo(1.2, duration * 0.6, Easing.SpringOut),
            element.RotateTo(0, duration * 0.6, Easing.SpringOut)
        };
        
        await Task.WhenAll(tasks);
        await element.ScaleTo(1, duration * 0.4, Easing.CubicOut);
    }
}
