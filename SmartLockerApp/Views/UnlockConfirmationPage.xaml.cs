namespace SmartLockerApp.Views;

public partial class UnlockConfirmationPage : ContentPage
{
    public UnlockConfirmationPage()
    {
        InitializeComponent();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void ContinueButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//OpenLockerPage");
    }

    private async void ViewSessionButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ActiveSessionPage");
    }

    private async void BackToHomeButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
}
