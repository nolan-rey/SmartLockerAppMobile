using SmartLockerApp.ViewModels;

namespace SmartLockerApp.Views;

public partial class HistoryPage : ContentPage
{
    public HistoryPage(HistoryPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is HistoryPageViewModel viewModel)
        {
            await viewModel.LoadDataCommand.ExecuteAsync(null);
        }
    }
}
