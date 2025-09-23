using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

[QueryProperty(nameof(SessionId), "sessionId")]
[QueryProperty(nameof(Action), "action")]
public partial class OpenLockerPage : ContentPage
{
    private readonly AppStateService _appState = AppStateService.Instance;
    public string SessionId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // "retrieve" pour récupération

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
        
        if (Action == "retrieve")
        {
            StatusMessage.Text = "Récupérez vos affaires et fermez le casier";
        }
        else
        {
            StatusMessage.Text = "Vous pouvez maintenant déposer vos affaires";
        }
        
        LoadingIndicator.IsRunning = false;
        ProgressContainer.IsVisible = false;
        
        // Show instructions and actions
        InstructionsContainer.IsVisible = true;
        ActionContainer.IsVisible = true;
    }

    private async void ConfirmRetrievalButton_Clicked(object sender, EventArgs e)
    {
        if (Action == "retrieve")
        {
            // Terminer la session et naviguer vers le reçu de paiement
            if (!string.IsNullOrEmpty(SessionId))
            {
                var result = await _appState.EndSessionAsync(SessionId);
                if (result.Success)
                {
                    // Naviguer vers PaymentPage avec action=receipt pour afficher le reçu
                    await Shell.Current.GoToAsync($"//PaymentPage?sessionId={SessionId}&action=receipt");
                }
                else
                {
                    await DisplayAlert("Erreur", result.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Erreur", "Session introuvable", "OK");
                await Shell.Current.GoToAsync("//HomePage");
            }
        }
        else
        {
            // Pour le dépôt initial, naviguer directement vers la page de casier ouvert
            if (!string.IsNullOrEmpty(SessionId))
            {
                await Shell.Current.GoToAsync($"//LockerOpenedPage?sessionId={SessionId}");
            }
            else
            {
                await DisplayAlert("Erreur", "Session introuvable", "OK");
                await Shell.Current.GoToAsync("//HomePage");
            }
        }
    }

    private async void NeedHelpButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HelpTutorialPage");
    }
}
