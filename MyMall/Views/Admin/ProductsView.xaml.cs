using System.Windows.Controls;
using MyMall.ViewModels.Admin;

namespace MyMall.Views.Admin;

public partial class ProductsView : UserControl
{
    public ProductsView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ProductsViewModel vm)
        {
            await vm.LoadAsync();
        }
    }
}