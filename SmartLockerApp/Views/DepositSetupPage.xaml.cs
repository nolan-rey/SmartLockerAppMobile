using SmartLockerApp.ViewModels;

namespace SmartLockerApp.Views;

public partial class DepositSetupPage : ContentPage
{
    public DepositSetupPage(DepositSetupPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is DepositSetupPageViewModel viewModel)
        {
            await viewModel.LoadDataCommand.ExecuteAsync(null);
        }
    }
}
