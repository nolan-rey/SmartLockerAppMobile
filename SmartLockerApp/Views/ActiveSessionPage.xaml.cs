namespace SmartLockerApp.Views;

public partial class ActiveSessionPage : ContentPage
{
    public ActiveSessionPage()
    {
        InitializeComponent();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OpenLockerButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//UnlockInstructionsPage");
    }

    private async void EndSessionButton_Clicked(object sender, EventArgs e)
    {
        // TODO: Implement end session logic
        await Shell.Current.GoToAsync("//PaymentPage");
    }
}
