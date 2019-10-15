using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Kmd.Logic.Identity.Examples.AdminApp.Domain;
using Kmd.Logic.Identity.Examples.AdminApp.Services;
using Microsoft.Identity.Client;

namespace Kmd.Logic.Identity.Examples.AdminApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private AuthenticationResult _authResult;
        private readonly DateService _dateService;
        private readonly TodoService _todoService;

        protected List<CalendarEvent> Dates { get; set; }
        protected List<Todo> Todos { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            _dateService = new DateService();
            _todoService = new TodoService();
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            var previousCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
            _authResult = null;
            var app = App.PublicClientApp;
            IEnumerable<IAccount> accounts = await App.PublicClientApp.GetAccountsAsync();
            try
            {
                ResultText.Text = "";
                _authResult = await (app as PublicClientApplication).AcquireTokenInteractive(App.ApiScopes)
                    .WithParentActivityOrWindow(new WindowInteropHelper(this).Handle)
                    .WithAccount(GetAccountByPolicy(accounts, App.PolicySignUpSignIn))
                    .ExecuteAsync();

                UpdateSignInState(true);
                ResultText.Text = "You are signed in";
            }
            catch (MsalException ex)
            {
                ResultText.Text = $"Error Acquiring Token:{Environment.NewLine}{ex}";
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Users:{string.Join(",", accounts.Select(u => u.Username))}{Environment.NewLine}Error Acquiring Token:{Environment.NewLine}{ex}";
            }

            Mouse.OverrideCursor = previousCursor;
        }

        private async void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IAccount> accounts = await App.PublicClientApp.GetAccountsAsync();
            try
            {
                while (accounts.Any())
                {
                    await App.PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                    accounts = await App.PublicClientApp.GetAccountsAsync();
                }

                UpdateSignInState(false);
            }
            catch (MsalException ex)
            {
                ResultText.Text = $"Error signing-out user: {ex.Message}";
            }
        }

        private void UpdateSignInState(bool signedIn)
        {
            if (signedIn)
            {
                dataGridsTab.Visibility = Visibility.Visible;
                SignOutButton.Visibility = Visibility.Visible;
                SignInButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ResultText.Text = "";
                dataGridsTab.Visibility = Visibility.Collapsed;
                SignOutButton.Visibility = Visibility.Collapsed;
                SignInButton.Visibility = Visibility.Visible;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var previousCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                var app = App.PublicClientApp;
                IEnumerable<IAccount> accounts = await App.PublicClientApp.GetAccountsAsync();

                _authResult = await app.AcquireTokenSilent(App.ApiScopes,
                        GetAccountByPolicy(accounts, App.PolicySignUpSignIn))
                    .ExecuteAsync();

                UpdateSignInState(true);
                ResultText.Text = "You are signed in";
            }
            catch (MsalUiRequiredException)
            {
                // Ignore, user will need to sign in interactively.
                ResultText.Text = "You need to sign-in";
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Error Acquiring Token Silently:{Environment.NewLine}{ex}";
            }
            Mouse.OverrideCursor = previousCursor;
        }

        private IAccount GetAccountByPolicy(IEnumerable<IAccount> accounts, string policy)
        {
            foreach (var account in accounts)
            {
                string accountIdentifier = account.HomeAccountId.ObjectId.Split('.')[0];
                if (accountIdentifier.EndsWith(policy.ToLower())) return account;
            }

            return null;
        }

        public void Dispose()
        {
            _dateService?.Dispose();
            _todoService?.Dispose();
        }

        private async void RefreshDates_OnClick(object sender, RoutedEventArgs e)
        {
            datesGrid.ItemsSource = await _dateService.GetDates(_authResult.AccessToken);
        }

        private async void DeleteDates_OnClick(object sender, RoutedEventArgs e)
        {
            if (await _dateService.DeleteAllDates(_authResult.AccessToken))
            {
                datesGrid.ItemsSource = await _dateService.GetDates(_authResult.AccessToken);
                ResultText.Text = "Successfully deleted dates";
            }
            else
            {
                ResultText.Text = "Failed to delete dates...";
            }
        }

        private async void RefreshTodos_OnClick(object sender, RoutedEventArgs e)
        {
            todosGrid.ItemsSource = await _todoService.GetTodos(_authResult.AccessToken);
        }

        private async void DeleteTodos_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserId.Text))
            {
                ResultText.Text = "You need to enter a UserId";
                return;
            }

            if (await _todoService.DeleteTodosForUser(_authResult.AccessToken, txtUserId.Text))
            {
                todosGrid.ItemsSource = await _todoService.GetTodos(_authResult.AccessToken);
                ResultText.Text = $"Successfully deleted todos for user {txtUserId.Text}";
                txtUserId.Text = string.Empty;
            }
            else
            {
                ResultText.Text = "Failed to delete todos for user...";
            }
        }
    }
}
