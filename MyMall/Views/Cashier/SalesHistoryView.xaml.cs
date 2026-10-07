using System.Windows.Controls;
using MyMall.ViewModels.Cashier;

namespace MyMall.Views.Cashier;

public partial class SalesHistoryView : UserControl
{
    public SalesHistoryView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is SalesHistoryViewModel vm)
        {
            await vm.LoadAsync();
        }
    }
}