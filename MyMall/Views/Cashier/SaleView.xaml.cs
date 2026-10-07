using System.Windows.Controls;
using System.Windows.Input;
using MyMall.ViewModels.Cashier;

namespace MyMall.Views.Cashier;

public partial class SaleView : UserControl
{
    public SaleView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is SaleViewModel vm)
        {
            await vm.LoadAsync();
        }
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is SaleViewModel vm)
        {
            vm.AddToCartCommand.Execute(null);
        }
    }
}