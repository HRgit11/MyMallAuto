using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MyMall.Services.DTOs;
using MyMall.Services.Interfaces;
using MyMall.ViewModels.Base;
using System.Collections.ObjectModel;

namespace MyMall.ViewModels.Admin;

public partial class ProductsViewModel : ViewModelBase
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ObservableCollection<ProductDto> Products { get; } = new();
    public ObservableCollection<CategoryDto> Categories { get; } = new();

    private ProductDto? _selectedProduct;
    public ProductDto? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (SetProperty(ref _selectedProduct, value))
            {
                OnPropertyChanged(nameof(IsProductSelected));
            }
        }
    }

    public bool IsProductSelected => SelectedProduct != null;

    private string _searchQuery = string.Empty;
    public string SearchQuery
    {
        get => _searchQuery;
        set => SetProperty(ref _searchQuery, value);
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

    public ProductsViewModel(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();

            Products.Clear();
            foreach (var p in products) Products.Add(p);

            Categories.Clear();
            foreach (var c in categories) Categories.Add(c);
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
    private async Task SearchAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var products = await _productService.SearchAsync(SearchQuery);

            Products.Clear();
            foreach (var p in products) Products.Add(p);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка поиска: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchQuery = string.Empty;
        _ = LoadAsync();
    }

    [RelayCommand]
    private void SelectProduct(ProductDto? product)
    {
        SelectedProduct = product;
    }
    [RelayCommand]
    private void AddProduct()
    {
        var editVm = App.Services.GetRequiredService<ProductEditViewModel>();
        _ = editVm.LoadCategoriesAsync();

        var window = new Views.Admin.ProductEditWindow(editVm);
        window.Owner = System.Windows.Application.Current.MainWindow;

        editVm.SaveCompleted += async () =>
        {
            await LoadAsync();
        };

        window.ShowDialog();
    }

    [RelayCommand]
    private void EditProduct()
    {
        if (SelectedProduct == null) return;

        var editVm = App.Services.GetRequiredService<ProductEditViewModel>();
        _ = editVm.LoadCategoriesAsync().ContinueWith(_ =>
        {
            _ = editVm.LoadProductAsync(SelectedProduct.Id);
        });

        var window = new Views.Admin.ProductEditWindow(editVm);
        window.Owner = System.Windows.Application.Current.MainWindow;

        editVm.SaveCompleted += async () =>
        {
            await LoadAsync();
        };

        window.ShowDialog();
    }

    [RelayCommand]
    private async Task DeleteProductAsync()
    {
        if (SelectedProduct == null) return;

        var result = System.Windows.MessageBox.Show(
            $"Удалить товар '{SelectedProduct.Name}'?",
            "Подтверждение",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result != System.Windows.MessageBoxResult.Yes) return;

        try
        {
            await _productService.DeleteAsync(SelectedProduct.Id);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка удаления: {ex.Message}";
        }
    }
}