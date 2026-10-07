using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMall.Services.DTOs;
using MyMall.Services.Interfaces;
using MyMall.ViewModels.Base;

namespace MyMall.ViewModels.Cashier;

public partial class SalesHistoryViewModel : ViewModelBase
{
    private readonly ISaleService _saleService;
    private readonly IAuthService _authService;

    public ObservableCollection<SaleDto> Sales { get; } = new();

    private DateTime _dateFrom = DateTime.Today.AddDays(-7);
    public DateTime DateFrom
    {
        get => _dateFrom;
        set => SetProperty(ref _dateFrom, value);
    }

    private DateTime _dateTo = DateTime.Today;
    public DateTime DateTo
    {
        get => _dateTo;
        set => SetProperty(ref _dateTo, value);
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public SalesHistoryViewModel(ISaleService saleService, IAuthService authService)
    {
        _saleService = saleService;
        _authService = authService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var from = DateTime.SpecifyKind(DateFrom.Date, DateTimeKind.Utc);
            var to = DateTime.SpecifyKind(DateTo.Date.AddDays(1).AddSeconds(-1), DateTimeKind.Utc);

            var sales = await _saleService.GetByDateRangeAsync(from, to);
            var user = _authService.CurrentUser;

            var filtered = sales
                .Where(s => user == null || s.CashierName == user.FullName)
                .ToList();

            Sales.Clear();
            foreach (var s in filtered) Sales.Add(s);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ApplyFilterAsync()
    {
        await LoadAsync();
    }
}