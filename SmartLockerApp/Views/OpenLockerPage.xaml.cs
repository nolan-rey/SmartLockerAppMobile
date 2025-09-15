namespace SmartLockerApp.Views;

public partial class OpenLockerPage : ContentPage
{
    public OpenLockerPage()
    {
        InitializeComponent();
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
        await Shell.Current.GoToAsync("//PaymentPage");
    }

    private async void NeedHelpButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HelpTutorialPage");
    }
}
