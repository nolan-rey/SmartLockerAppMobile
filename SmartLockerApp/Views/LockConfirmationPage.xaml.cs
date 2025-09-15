namespace SmartLockerApp.Views;

public partial class LockConfirmationPage : ContentPage
{
    public LockConfirmationPage()
    {
        InitializeComponent();
    }

    private async void HomeButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
}
