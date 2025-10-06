using SmartLockerApp.ViewModels;

namespace SmartLockerApp.Views;

public partial class ActiveSessionPage : ContentPage
{
    public ActiveSessionPage(ActiveSessionPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is ActiveSessionPageViewModel viewModel)
        {
            await viewModel.LoadDataCommand.ExecuteAsync(null);
        }
    }
}
