namespace SmartLockerApp.Views;

public partial class HistoryPage : ContentPage
{
    private string currentFilter = "All";

    public HistoryPage()
    {
        InitializeComponent();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void FilterButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        
        // Reset all button styles
        ResetFilterButtons();
        
        // Set active button style
        button.BackgroundColor = Color.FromArgb("#2563EB");
        button.TextColor = Colors.White;
        
        // Update current filter
        currentFilter = button.Text;
        
        // TODO: Implement filtering logic
        FilterHistory(currentFilter);
    }

    private void ResetFilterButtons()
    {
        var buttons = new[] { AllButton, CompletedButton, ActiveButton };
        
        foreach (var btn in buttons)
        {
            btn.BackgroundColor = Colors.Transparent;
            btn.TextColor = Color.FromArgb("#64748B");
        }
    }

    private void FilterHistory(string filter)
    {
        // TODO: Implement actual filtering logic based on data source
        // For now, this is just a placeholder
        switch (filter)
        {
            case "Tout":
                // Show all sessions
                break;
            case "Terminées":
                // Show only completed sessions
                break;
            case "Actives":
                // Show only active sessions
                break;
        }
    }

    private async void LoadMoreButton_Clicked(object sender, EventArgs e)
    {
        // TODO: Implement load more functionality
        await DisplayAlert("Chargement", "Chargement de plus de sessions...", "OK");
    }
}
