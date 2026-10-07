using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMall.Services.DTOs;
using MyMall.Services.Interfaces;
using MyMall.ViewModels.Base;

namespace MyMall.ViewModels.Admin;

public partial class ProductEditViewModel : ViewModelBase
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ObservableCollection<CategoryDto> Categories { get; } = new();

    private int? _productId;

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private string? _barcode;
    public string? Barcode
    {
        get => _barcode;
        set => SetProperty(ref _barcode, value);
    }

    private decimal _price;
    public decimal Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }

    private decimal _purchasePrice;
    public decimal PurchasePrice
    {
        get => _purchasePrice;
        set => SetProperty(ref _purchasePrice, value);
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    private CategoryDto? _selectedCategory;
    public CategoryDto? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private string _title = "Добавление товара";
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public event Action? SaveCompleted;

    public ProductEditViewModel(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task LoadCategoriesAsync()
    {
        var categories = await _categoryService.GetAllAsync();
        Categories.Clear();
        foreach (var c in categories) Categories.Add(c);
    }

    public async Task LoadProductAsync(int productId)
    {
        _productId = productId;
        Title = "Редактирование товара";

        var product = await _productService.GetByIdAsync(productId);
        if (product == null)
        {
            ErrorMessage = "Товар не найден";
            return;
        }

        Name = product.Name;
        Description = product.Description;
        Barcode = product.Barcode;
        Price = product.Price;
        PurchasePrice = product.PurchasePrice;
        Quantity = product.Quantity;
        SelectedCategory = Categories.FirstOrDefault(c => c.Id == product.CategoryId);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "Введите название товара";
            return;
        }

        if (SelectedCategory == null)
        {
            ErrorMessage = "Выберите категорию";
            return;
        }

        if (Price < 0 || PurchasePrice < 0)
        {
            ErrorMessage = "Цена не может быть отрицательной";
            return;
        }

        try
        {
            var dto = new CreateProductDto
            {
                Name = Name,
                Description = Description,
                Barcode = Barcode,
                Price = Price,
                PurchasePrice = PurchasePrice,
                Quantity = Quantity,
                CategoryId = SelectedCategory.Id
            };

            if (_productId.HasValue)
            {
                await _productService.UpdateAsync(_productId.Value, dto);
            }
            else
            {
                await _productService.CreateAsync(dto);
            }

            SaveCompleted?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        SaveCompleted?.Invoke();
    }
}