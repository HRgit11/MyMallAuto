using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MyMall.Services.Interfaces;
using MyMall.ViewModels.Base;

namespace MyMall.ViewModels.Admin;

public partial class AdminMainViewModel : ViewModelBase
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

    public AdminMainViewModel(IAuthService authService)
    {
        _authService = authService;
        UserName = authService.CurrentUser?.FullName ?? "Администратор";
    }

    [RelayCommand]
    private void ShowProducts()
    {
        CurrentViewModel = App.Services.GetRequiredService<ProductsViewModel>();
    }

    [RelayCommand]
    private void ShowCategories()
    {
        CurrentViewModel = App.Services.GetRequiredService<CategoriesViewModel>();
    }

    [RelayCommand]
    private void ShowReports()
    {
        CurrentViewModel = App.Services.GetRequiredService<ReportsViewModel>();
    }

    [RelayCommand]
    private void Logout()
    {
        _authService.Logout();

        var loginWindow = new Views.LoginWindow();
        loginWindow.Show();

        foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
        {
            if (window is Views.Admin.AdminMainWindow)
            {
                window.Close();
                break;
            }
        }
    }
}