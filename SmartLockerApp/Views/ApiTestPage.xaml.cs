using SmartLockerApp.Services;
using System.Text;

namespace SmartLockerApp.Views;

public partial class ApiTestPage : ContentPage
{
    private readonly SmartLockerIntegratedService _smartLockerService;
    private readonly StringBuilder _logs = new();

    public ApiTestPage()
    {
        InitializeComponent();
        
        // Récupérer le service depuis l'injection de dépendances
        _smartLockerService = Application.Current?.Handler?.MauiContext?.Services
            .GetService<SmartLockerIntegratedService>()
            ?? throw new InvalidOperationException("SmartLockerIntegratedService non trouvé");
    }

    private void AddLog(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        _logs.AppendLine($"[{timestamp}] {message}");
        LogsLabel.Text = _logs.ToString();
    }

    private async void TestLogin_Clicked(object sender, EventArgs e)
    {
        TestLoginButton.IsEnabled = false;
        LoginResultLabel.Text = "⏳ Test en cours...";
        AddLog("🔐 Démarrage test login admin...");

        try
        {
            var success = await _smartLockerService.LoginAsync("SaintMichel", "ITcampus");
            
            if (success)
            {
                LoginResultLabel.Text = "✅ Connexion réussie !";
                LoginResultLabel.TextColor = Color.FromArgb("#10B981");
                AddLog("✅ Login admin réussi");
            }
            else
            {
                LoginResultLabel.Text = "❌ Échec de connexion";
                LoginResultLabel.TextColor = Color.FromArgb("#EF4444");
                AddLog("❌ Login admin échoué");
            }
        }
        catch (Exception ex)
        {
            LoginResultLabel.Text = $"❌ Erreur: {ex.Message}";
            LoginResultLabel.TextColor = Color.FromArgb("#EF4444");
            AddLog($"❌ Exception: {ex.Message}");
        }
        finally
        {
            TestLoginButton.IsEnabled = true;
        }
    }

    private async void TestCreateUser_Clicked(object sender, EventArgs e)
    {
        TestCreateUserButton.IsEnabled = false;
        CreateUserResultLabel.Text = "⏳ Test en cours...";
        AddLog("👤 Démarrage test création utilisateur...");

        try
        {
            var email = TestEmailEntry.Text?.Trim();
            var password = TestPasswordEntry.Text?.Trim();
            var name = TestNameEntry.Text?.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name))
            {
                CreateUserResultLabel.Text = "❌ Remplissez tous les champs";
                CreateUserResultLabel.TextColor = Color.FromArgb("#EF4444");
                return;
            }

            var username = email.Split('@')[0];
            AddLog($"📝 Création utilisateur: {username} / {email}");

            var (success, message, user) = await _smartLockerService.CreateUserAsync(
                username, password, email, name);
            
            if (success && user != null)
            {
                CreateUserResultLabel.Text = $"✅ {message}\nID: {user.Id}";
                CreateUserResultLabel.TextColor = Color.FromArgb("#10B981");
                AddLog($"✅ Utilisateur créé: ID={user.Id}, Name={user.Name}");
            }
            else
            {
                CreateUserResultLabel.Text = $"❌ {message}";
                CreateUserResultLabel.TextColor = Color.FromArgb("#EF4444");
                AddLog($"❌ Échec: {message}");
            }
        }
        catch (Exception ex)
        {
            CreateUserResultLabel.Text = $"❌ Erreur: {ex.Message}";
            CreateUserResultLabel.TextColor = Color.FromArgb("#EF4444");
            AddLog($"❌ Exception: {ex.Message}");
        }
        finally
        {
            TestCreateUserButton.IsEnabled = true;
        }
    }

    private async void TestLockers_Clicked(object sender, EventArgs e)
    {
        TestLockersButton.IsEnabled = false;
        LockersResultLabel.Text = "⏳ Test en cours...";
        AddLog("🔒 Démarrage test récupération casiers...");

        try
        {
            var lockers = await _smartLockerService.GetAvailableLockersAsync();
            
            if (lockers != null && lockers.Count > 0)
            {
                LockersResultLabel.Text = $"✅ {lockers.Count} casier(s) disponible(s):\n" +
                    string.Join("\n", lockers.Select(l => $"- {l.Name} (ID: {l.Id})"));
                LockersResultLabel.TextColor = Color.FromArgb("#10B981");
                AddLog($"✅ {lockers.Count} casiers récupérés");
                foreach (var locker in lockers)
                {
                    AddLog($"  - {locker.Name} (ID: {locker.Id}, Status: {locker.Status})");
                }
            }
            else
            {
                LockersResultLabel.Text = "⚠️ Aucun casier disponible";
                LockersResultLabel.TextColor = Color.FromArgb("#F59E0B");
                AddLog("⚠️ Liste vide");
            }
        }
        catch (Exception ex)
        {
            LockersResultLabel.Text = $"❌ Erreur: {ex.Message}";
            LockersResultLabel.TextColor = Color.FromArgb("#EF4444");
            AddLog($"❌ Exception: {ex.Message}");
        }
        finally
        {
            TestLockersButton.IsEnabled = true;
        }
    }

    private void ClearLogs_Clicked(object sender, EventArgs e)
    {
        _logs.Clear();
        LogsLabel.Text = "Logs effacés...";
        AddLog("🗑️ Logs effacés");
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
