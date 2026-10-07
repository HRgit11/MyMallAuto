using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MyMall.ViewModels.Admin;

namespace MyMall.Views.Admin;

public partial class AdminMainWindow : Window
{
    public AdminMainWindow() : this(App.Services.GetRequiredService<AdminMainViewModel>())
    {
    }

    public AdminMainWindow(AdminMainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}