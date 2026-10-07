using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMall.Services.DTOs;
using MyMall.Services.Interfaces;
using MyMall.ViewModels.Base;

namespace MyMall.ViewModels.Admin;

public partial class ReportsViewModel : ViewModelBase
{
    private readonly ISaleService _saleService;

    public ObservableCollection<SaleDto> Sales { get; } = new();

    private DateTime _dateFrom = DateTime.Today;
    public DateTime DateFrom
    {
        get => _dateFrom;
        set
        {
            if (SetProperty(ref _dateFrom, value))
            {
                OnPropertyChanged(nameof(DateFromDate));
            }
        }
    }

    public DateTime DateFromDate
    {
        get => _dateFrom.Date;
        set
        {
            if (SetProperty(ref _dateFrom, value))
            {
                OnPropertyChanged(nameof(DateFrom));
            }
        }
    }

    private DateTime _dateTo = DateTime.Today.AddDays(1);
    public DateTime DateTo
    {
        get => _dateTo;
        set
        {
            if (SetProperty(ref _dateTo, value))
            {
                OnPropertyChanged(nameof(DateToDate));
            }
        }
    }

    public DateTime DateToDate
    {
        get => _dateTo.Date;
        set
        {
            if (SetProperty(ref _dateTo, value))
            {
                OnPropertyChanged(nameof(DateTo));
            }
        }
    }

    private decimal _totalRevenue;
    public decimal TotalRevenue
    {
        get => _totalRevenue;
        set => SetProperty(ref _totalRevenue, value);
    }

    private int _totalSales;
    public int TotalSales
    {
        get => _totalSales;
        set => SetProperty(ref _totalSales, value);
    }

    private decimal _averageCheck;
    public decimal AverageCheck
    {
        get => _averageCheck;
        set => SetProperty(ref _averageCheck, value);
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

    public ReportsViewModel(ISaleService saleService)
    {
        _saleService = saleService;
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
            var salesList = sales.ToList();

            Sales.Clear();
            foreach (var s in salesList) Sales.Add(s);

            TotalSales = salesList.Count;
            TotalRevenue = salesList
                .Where(s => s.Status == Enums.SaleStatus.Completed)
                .Sum(s => s.TotalAmount);

            AverageCheck = TotalSales > 0 ? TotalRevenue / TotalSales : 0;
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

    [RelayCommand]
    private async Task TodayAsync()
    {
        DateFrom = DateTime.Today;
        DateTo = DateTime.Today;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task WeekAsync()
    {
        DateFrom = DateTime.Today.AddDays(-7);
        DateTo = DateTime.Today;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task MonthAsync()
    {
        DateFrom = DateTime.Today.AddDays(-30);
        DateTo = DateTime.Today;
        await LoadAsync();
    }
}