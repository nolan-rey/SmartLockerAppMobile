using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

[QueryProperty(nameof(SessionId), "sessionId")]
public partial class OpenLockerPage : ContentPage
{
    private readonly AppStateService _appState = AppStateService.Instance;
    public string SessionId { get; set; } = string.Empty;

    public OpenLockerPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartOpeningProcess();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void StartOpeningProcess()
    {
        // Simulate opening process
        await Task.Delay(3000);
        
        // Update UI to show success
        StatusIcon.Text = "✅";
        StatusTitle.Text = "Casier ouvert !";
        StatusMessage.Text = "Vous pouvez maintenant récupérer vos affaires";
        LoadingIndicator.IsRunning = false;
        ProgressContainer.IsVisible = false;
        
        // Show instructions and actions
        InstructionsContainer.IsVisible = true;
        ActionContainer.IsVisible = true;
    }

    private async void ConfirmRetrievalButton_Clicked(object sender, EventArgs e)
    {
        // Naviguer vers les instructions de verrouillage avec l'ID de session
        await Shell.Current.GoToAsync($"//LockInstructionsPage?sessionId={SessionId}");
    }

    private async void NeedHelpButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HelpTutorialPage");
    }
}
