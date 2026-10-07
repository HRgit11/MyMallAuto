using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MyMall.Services.Interfaces;
using MyMall.ViewModels.Base;
namespace MyMall.ViewModels.Cashier;

public partial class CashierMainViewModel : ViewModelBase
{
    private readonly IAuthService _authService;

    private ViewModelBase? _currentViewModel;
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    private string _userName = string.Empty;
    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    public CashierMainViewModel(IAuthService authService)
    {
        _authService = authService;
        UserName = authService.CurrentUser?.FullName ?? "Кассир";
    }

    [RelayCommand]
    private void ShowSale()
    {
        CurrentViewModel = App.Services.GetRequiredService<SaleViewModel>();
    }

    [RelayCommand]
    private void ShowSalesHistory()
    {
        CurrentViewModel = App.Services.GetRequiredService<SalesHistoryViewModel>();
    }

    [RelayCommand]
    private void Logout()
    {
        _authService.Logout();

        var loginWindow = new Views.LoginWindow();
        loginWindow.Show();

        foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
        {
            if (window is Views.Cashier.CashierMainWindow)
            {
                window.Close();
                break;
            }
        }
    }
}