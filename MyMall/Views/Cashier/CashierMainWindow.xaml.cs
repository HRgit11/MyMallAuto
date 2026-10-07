using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MyMall.ViewModels.Cashier;

namespace MyMall.Views.Cashier;

public partial class CashierMainWindow : Window
{
    public CashierMainWindow() : this(App.Services.GetRequiredService<CashierMainViewModel>())
    {
    }

    public CashierMainWindow(CashierMainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}