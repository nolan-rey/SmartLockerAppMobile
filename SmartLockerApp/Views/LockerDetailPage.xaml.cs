using SmartLockerApp.ViewModels;
using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

public partial class LockerDetailPage : ContentPage
{
    public LockerDetailPage(LockerDetailPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await AnimationService.FadeInAsync(this);
        await AnimationService.SlideInFromBottomAsync(this.Content, 400);
        
        if (BindingContext is LockerDetailPageViewModel viewModel)
        {
            viewModel.LoadDataCommand.Execute(null);
        }
    }
}
