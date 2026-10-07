using System.Windows;
using MyMall.ViewModels.Admin;

namespace MyMall.Views.Admin;

public partial class ProductEditWindow : Window
{
    public ProductEditWindow(ProductEditViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.SaveCompleted += OnSaveCompleted;
    }

    private void OnSaveCompleted()
    {
        Close();
    }
}