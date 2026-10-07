using Microsoft.Extensions.DependencyInjection;
using MyMall.Services.Interfaces;
using MyMall.ViewModels;
using MyMall.Views.Admin;
using System.Windows;
using System.Windows.Controls;

namespace MyMall.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow() : this(App.Services.GetRequiredService<LoginViewModel>())
    {
    }

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        viewModel.LoginSuccess += OnLoginSuccess;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox pb)
        {
            _viewModel.Password = pb.Password;
        }
    }

    private void OnLoginSuccess()
    {
        var authService = App.Services.GetRequiredService<IAuthService>();
        var user = authService.CurrentUser;

        if (user == null) return;

        Window nextWindow;

        if (user.Role == Enums.UserRole.Admin)
        {
            nextWindow = App.Services.GetRequiredService<MyMall.Views.Admin.AdminMainWindow>();
        }
        else
        {
            nextWindow = App.Services.GetRequiredService<MyMall.Views.Cashier.CashierMainWindow>();
        }

        nextWindow.Show();
        Close();
    }
}