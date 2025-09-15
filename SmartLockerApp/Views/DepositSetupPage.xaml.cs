namespace SmartLockerApp.Views;

public partial class DepositSetupPage : ContentPage
{
    private string selectedDuration = "1 heure";
    private string selectedPrice = "4,00€";

    public DepositSetupPage()
    {
        InitializeComponent();
        
        // Set default selection (1 hour)
        UpdateSelection("1 heure", "4,00€");
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void OnOption30MinTapped(object sender, EventArgs e)
    {
        UpdateSelection("30 minutes", "2,50€");
        UpdateRadioButtons("30min");
    }

    private void OnOption1HourTapped(object sender, EventArgs e)
    {
        UpdateSelection("1 heure", "4,00€");
        UpdateRadioButtons("1hour");
    }

    private void OnOption2HoursTapped(object sender, EventArgs e)
    {
        UpdateSelection("2 heures", "7,00€");
        UpdateRadioButtons("2hours");
    }

    private void OnOption4HoursTapped(object sender, EventArgs e)
    {
        UpdateSelection("4 heures", "12,00€");
        UpdateRadioButtons("4hours");
    }

    private void UpdateSelection(string duration, string price)
    {
        selectedDuration = duration;
        selectedPrice = price;
        SelectedDurationText.Text = duration;
        SelectedPriceText.Text = price;
    }

    private void UpdateRadioButtons(string selected)
    {
        // Reset all borders and radios
        Option30Min.Stroke = Colors.Gray;
        Option1Hour.Stroke = Colors.Gray;
        Option2Hours.Stroke = Colors.Gray;
        Option4Hours.Stroke = Colors.Gray;

        Radio30Min.BackgroundColor = Colors.Transparent;
        Radio1Hour.BackgroundColor = Colors.Transparent;
        Radio2Hours.BackgroundColor = Colors.Transparent;
        Radio4Hours.BackgroundColor = Colors.Transparent;

        // Set selected option
        switch (selected)
        {
            case "30min":
                Option30Min.Stroke = Color.FromArgb("#3B82F6");
                Radio30Min.BackgroundColor = Color.FromArgb("#3B82F6");
                break;
            case "1hour":
                Option1Hour.Stroke = Color.FromArgb("#3B82F6");
                Radio1Hour.BackgroundColor = Color.FromArgb("#3B82F6");
                break;
            case "2hours":
                Option2Hours.Stroke = Color.FromArgb("#3B82F6");
                Radio2Hours.BackgroundColor = Color.FromArgb("#3B82F6");
                break;
            case "4hours":
                Option4Hours.Stroke = Color.FromArgb("#3B82F6");
                Radio4Hours.BackgroundColor = Color.FromArgb("#3B82F6");
                break;
        }
    }

    private async void ConfirmButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LockInstructionsPage");
    }
}
