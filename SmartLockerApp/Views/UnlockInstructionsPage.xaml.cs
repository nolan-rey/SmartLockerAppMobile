using SmartLockerApp.ViewModels;

namespace SmartLockerApp.Views;

public partial class UnlockInstructionsPage : ContentPage
{
    public UnlockInstructionsPage(UnlockInstructionsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is UnlockInstructionsPageViewModel viewModel)
        {
            await viewModel.LoadDataCommand.ExecuteAsync(null);
        }
    }
}
