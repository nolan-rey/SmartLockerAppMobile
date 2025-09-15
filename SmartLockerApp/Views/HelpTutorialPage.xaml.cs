namespace SmartLockerApp.Views;

public partial class HelpTutorialPage : ContentPage
{
    public HelpTutorialPage()
    {
        InitializeComponent();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void ContactSupportButton_Clicked(object sender, EventArgs e)
    {
        // TODO: Implement contact support functionality
        await DisplayAlert("Support", "Fonctionnalité de contact support à implémenter", "OK");
    }
}
