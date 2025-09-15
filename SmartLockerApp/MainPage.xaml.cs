namespace SmartLockerApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private async void OnSignupClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignupPage");
        }

        private async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ForgotPasswordPage");
        }

        private async void OnHomePageClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//HomePage");
        }

        private async void OnLockerDetailClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LockerDetailPage");
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//HelpTutorialPage");
        }

        private async void OnActiveSessionClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ActiveSessionPage");
        }

        private async void OnHistoryClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//HistoryPage");
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SettingsPage");
        }

        private async void OnPaymentClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//PaymentPage");
        }
    }
}
