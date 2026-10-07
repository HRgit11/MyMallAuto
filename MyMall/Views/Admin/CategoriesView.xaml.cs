using System.Windows.Controls;
using MyMall.ViewModels.Admin;

namespace MyMall.Views.Admin;

public partial class CategoriesView : UserControl
{
    public CategoriesView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is CategoriesViewModel vm)
        {
            await vm.LoadAsync();
        }
    }
}