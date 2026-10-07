using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMall.Enums;
using MyMall.Services.DTOs;
using MyMall.Services.Interfaces;
using MyMall.ViewModels.Base;

namespace MyMall.ViewModels.Cashier;

public partial class SaleViewModel : ViewModelBase
{
    private readonly IProductService _productService;
    private readonly ISaleService _saleService;
    private readonly IAuthService _authService;

    public ObservableCollection<ProductDto> Products { get; } = new();
    public ObservableCollection<CartItem> Cart { get; } = new();

    private string _searchQuery = string.Empty;
    public string SearchQuery
    {
        get => _searchQuery;
        set => SetProperty(ref _searchQuery, value);
    }

    private ProductDto? _selectedProduct;
    public ProductDto? SelectedProduct
    {
        get => _selectedProduct;
        set => SetProperty(ref _selectedProduct, value);
    }

    private PaymentMethod _paymentMethod = PaymentMethod.Cash;
    public PaymentMethod PaymentMethod
    {
        get => _paymentMethod;
        set => SetProperty(ref _paymentMethod, value);
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private string? _successMessage;
    public string? SuccessMessage
    {
        get => _successMessage;
        set => SetProperty(ref _successMessage, value);
    }

    public decimal TotalAmount => Cart.Sum(i => i.TotalPrice);

    public SaleViewModel(
        IProductService productService,
        ISaleService saleService,
        IAuthService authService)
    {
        _productService = productService;
        _saleService = saleService;
        _authService = authService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        ErrorMessage = null;

        try
        {
            var products = await _productService.GetAllAsync();

            Products.Clear();
            foreach (var p in products) Products.Add(p);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
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
    }

    [RelayCommand]
    private void AddToCart()
    {
        if (SelectedProduct == null) return;

        if (SelectedProduct.Quantity <= 0)
        {
            ErrorMessage = $"Товар '{SelectedProduct.Name}' закончился";
            return;
        }

        var existing = Cart.FirstOrDefault(i => i.ProductId == SelectedProduct.Id);
        if (existing != null)
        {
            if (existing.Quantity + 1 > SelectedProduct.Quantity)
            {
                ErrorMessage = $"Недостаточно товара '{SelectedProduct.Name}' на складе";
                return;
            }
            existing.Quantity++;
        }
        else
        {
            Cart.Add(new CartItem
            {
                ProductId = SelectedProduct.Id,
                ProductName = SelectedProduct.Name,
                Quantity = 1,
                UnitPrice = SelectedProduct.Price
            });
        }

        ErrorMessage = null;
        OnPropertyChanged(nameof(TotalAmount));
    }

    [RelayCommand]
    private void RemoveFromCart(CartItem? item)
    {
        if (item == null) return;

        Cart.Remove(item);
        OnPropertyChanged(nameof(TotalAmount));
    }

    [RelayCommand]
    private void ClearCart()
    {
        Cart.Clear();
        OnPropertyChanged(nameof(TotalAmount));
    }

    [RelayCommand]
    private async Task CompleteSaleAsync()
    {
        ErrorMessage = null;
        SuccessMessage = null;

        if (Cart.Count == 0)
        {
            ErrorMessage = "Корзина пуста";
            return;
        }

        var user = _authService.CurrentUser;
        if (user == null)
        {
            ErrorMessage = "Пользователь не авторизован";
            return;
        }

        try
        {
            var dto = new CreateSaleDto
            {
                UserId = user.Id,
                PaymentMethod = PaymentMethod,
                Items = Cart.Select(i => new CreateSaleItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            var sale = await _saleService.CreateSaleAsync(dto);

            SuccessMessage = $"Продажа #{sale.Id} на сумму {sale.TotalAmount:N2} ₽ успешно оформлена";
            Cart.Clear();
            OnPropertyChanged(nameof(TotalAmount));

            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка продажи: {ex.Message}";
        }
    }
}

public partial class CartItem : ObservableObject
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (SetProperty(ref _quantity, value))
            {
                OnPropertyChanged(nameof(TotalPrice));
            }
        }
    }

    public decimal TotalPrice => Quantity * UnitPrice;
}